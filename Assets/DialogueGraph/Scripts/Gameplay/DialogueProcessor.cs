using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Dennis.Tools.DialogueGraph.UI;
using Dennis.Tools.DialogueGraph.Data;
using Dennis.Tools.DialogueGraph.Event;

using Dennis.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueProcessor : DialogueFlowHandler
    {
        [SerializeField] private DialogueContainerDatabase _database;

        [SerializeField] private DialogueControllerUI _dialogueControllerUI;

        private DialogueNodeData _currentDialogueNodeData;

        private List<DialogueElementBase> _baseContainers;
        private int _currentIndex = 0;

        public void Initialize()
        {
            DialogueContainerLoader.Initialize(_database);

            _dialogueControllerUI.OnAnimationComplete += HandleDialogueFlow;
            Events.OnDialogueTriggered.Add(LoadAndStartDialogue);
        }

        private void OnDestroy()
        {
            _dialogueControllerUI.OnAnimationComplete -= HandleDialogueFlow;
            Events.OnDialogueTriggered.Remove(LoadAndStartDialogue);
        }

        /// <summary>
        /// Loads a DialogueContainer by its ID and starts the dialogue
        /// </summary>
        /// <param name="dialogueContainerId">The ID of the DialogueContainer to load</param>
        public void LoadAndStartDialogue(string dialogueContainerId)
        {
            // Load the dialogue container from resources
            DialogueContainer = DialogueContainerLoader.LoadDialogueContainer(dialogueContainerId);

            // Start the dialogue only if the container is successfully loaded
            if (DialogueContainer != null)
            {
                StartDialogue();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Failed to load DialogueContainer with ID: {dialogueContainerId}");
#endif
                // OnDialogueCompleted Event
                Events.OnDialogueCompleted.Publish();
            }
        }

        private void StartDialogue()
        {
            _dialogueControllerUI.ShowDialogueUI(true);
            ProcessNodeType(GetNextNode(DialogueContainer.StartNodeData));
        }

        private void ExecuteStartNode()
        {
            ProcessNodeType(GetNextNode(DialogueContainer.StartNodeData));
        }

        private void ProcessNodeType(BaseData baseNodeData)
        {
            switch (baseNodeData)
            {
                case StartData startNodeData:
                    ExecuteStartNode();
                    break;

                case EndNodeData endNodeData:
                    ExecuteEndNode(endNodeData);
                    break;

                case DialogueNodeData dialogueNodeData:
                    ExecuteDialogueNode(dialogueNodeData);
                    break;

                case EventNodeData eventNodeData:
                    ExecuteEventNode(eventNodeData);
                    break;

                case BranchNodeData branchNodeData:
                    ExecuteBranchNode(branchNodeData);
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogWarning($"Unhandled node type: {baseNodeData.GetType().Name}");
#endif
                    break;
            }
        }

        #region Event Node

        /// <summary>
        /// Executes event node
        /// </summary>
        /// <param name="eventNodeData">event node data</param>
        private void ExecuteEventNode(EventNodeData eventNodeData)
        {
            List<VariableOperationData> variableOperations = eventNodeData.VariableOperationDatas;

            // Publish all variable operations to notify relevant event handlers
            foreach (VariableOperationData operation in variableOperations)
            {
                Events.OnVariableOperationEvents.Publish(operation);
            }

            List<DialogueEventSO> dialogueEvents = eventNodeData.DialogueEventSOs;

            // all DialogueEventSOs execute Event
            foreach (DialogueEventSO eventSO in dialogueEvents)
            {
                eventSO.ExecuteEvent();
            }

            // Determine the next event node and process it
            ProcessNodeType(GetNextNode(eventNodeData));
        }

        #endregion Event Node

        #region Branch Node

        /// <summary>
        /// Executes branch node
        /// </summary>
        /// <param name="branchNodeData">The branch node data containing conditions and next node references</param>
        private void ExecuteBranchNode(BranchNodeData branchNodeData)
        {
            string nextNodeGuid = IsConditionFulfilled(branchNodeData.ConditionDatas)
                ? branchNodeData.TrueGuidNode
                : branchNodeData.FalseGuidNode;

            ProcessNodeType(GetNodeByGuid(nextNodeGuid));
        }

        private bool IsConditionFulfilled(List<ConditionData> conditionDatas)
        {
            if (conditionDatas == null) return true;

            // Iterate through all conditions
            foreach (ConditionData conditionData in conditionDatas)
            {
                // If any condition is NOT met, execute the False and return early
                if (!VariableManager.EvaluateCondition(conditionData))
                {
                    return false;
                }
            }

            // If all conditions are met, execute the True branch
            return true;
        }

        #endregion Branch Node

        #region Dialogue Node

        private void ExecuteDialogueNode(DialogueNodeData dialogueNodeData)
        {
            InitDialogueData(dialogueNodeData);

            _baseContainers.Sort(delegate (DialogueElementBase x, DialogueElementBase y)
            {
                return x.OrderIndex.CompareTo(y.OrderIndex);
            });

            ProcessDialogue();
        }

        private void InitDialogueData(DialogueNodeData dialogueNodeData)
        {
            _currentDialogueNodeData = dialogueNodeData;

            _baseContainers = dialogueNodeData.AllDialogueElements;
            _currentIndex = 0;
        }


        private void ProcessDialogue()
        {
            SoundManager.Instance.StopSE();

            for (int i = _currentIndex; i < _baseContainers.Count; i++)
            {
                _currentIndex = i + 1;
                if (_baseContainers[i] is DialogueImagesData)
                {
                    DialogueImagesData tmp = _baseContainers[i] as DialogueImagesData;
                    _dialogueControllerUI.SetImage(tmp.Sprite_Left, tmp.Sprite_Right, tmp.SpeakerTypeLeft, tmp.SpeakerTypeRight);
                }
                else if (_baseContainers[i] is DialogueNameData)
                {
                    DialogueNameData tmp = _baseContainers[i] as DialogueNameData;
                    _dialogueControllerUI.SetName(tmp.Name);
                }
                else if (_baseContainers[i] is DialogueBoxData)
                {
                    DialogueBoxData tmp = _baseContainers[i] as DialogueBoxData;
                    _dialogueControllerUI.SetContentText(tmp.Text);

                    // TODO: Refactor Manager to handle audio playback more effectively
                    SoundManager.Instance.PlaySE(tmp.AudioClip);
                    break;
                }
            }
        }

        private void HandleDialogueFlow()
        {
            SoundManager.Instance.StopSE();

            bool isDialogueEnd = _currentIndex == _baseContainers.Count && _currentDialogueNodeData.DialogueDataPorts.Count == 0;
            bool hasChoices = _currentIndex == _baseContainers.Count;

            // Check if the dialogue has ended (no further choices available)
            if (isDialogueEnd)
            {
                // Automatically proceed to the next node
                UnityAction continueAction = () => ProcessNodeType(GetNextNode(_currentDialogueNodeData));
                _dialogueControllerUI.ConfigureContinueButton(continueAction);
            }
            // Check if there are selectable choices
            else if (hasChoices)
            {
                List<DialogueOption> dialogueOptions = new List<DialogueOption>();

                // Iterate through all available choices in the current dialogue node
                foreach (DialogueDataPort choice in _currentDialogueNodeData.DialogueDataPorts)
                {
                    ProcessChoiceOption(choice.InputGuid, dialogueOptions);
                }

                // Display the dialogue choice buttons
                _dialogueControllerUI.SetupDialogueOptions(dialogueOptions);
            }
            // Continue the current dialogue
            else
            {
                UnityAction continueAction = () => ProcessDialogue();
                _dialogueControllerUI.ConfigureContinueButton(continueAction);
            }
        }

        /// <summary>
        /// Processes a choice option based on the given node GUID and adds it to the dialogue options.
        /// </summary>
        /// <param name="guidID">The GUID of the choice node</param>
        /// <param name="dialogueOptions">The list to store the processed dialogue options</param>
        private void ProcessChoiceOption(string guidID, List<DialogueOption> dialogueOptions)
        {
            // Retrieve the ChoiceNodeData
            if (!(GetNodeByGuid(guidID) is ChoiceNodeData choiceNode))
            {
#if UNITY_EDITOR
                Debug.LogError($"[ProcessChoiceOption] Node with GUID '{guidID}' is not a ChoiceNodeData.");
#endif
                return;
            }

            // Evaluate the condition for this choice
            bool isConditionMet = IsConditionFulfilled(choiceNode.ConditionDatas);

            // Define action to process the next node
            UnityAction onChoiceSelected = () => ProcessNodeType(GetNextNode(choiceNode));

            // Create and configure the dialogue option
            DialogueOption dialogueOption = new DialogueOption
            {
                FailAction = choiceNode.ChoiceNodeFailAction,
                Text = choiceNode.Text,
                OnChoiceSelected = onChoiceSelected,
                ConditionCheck = isConditionMet
            };

            // Add the configured option to the list
            dialogueOptions.Add(dialogueOption);
        }


        #endregion Dialogue Node

        #region End Node

        private void ExecuteEndNode(EndNodeData endNodeData)
        {
            switch (endNodeData.EndNodeType)
            {
                case EndNodeType.End:
                    // Close the DialogueUI 
                    _dialogueControllerUI.ShowDialogueUI(false);
                    break;

                case EndNodeType.RetrunToStart:
                    // Return to the Start Node
                    ExecuteStartNode();
                    break;

                case EndNodeType.Repeat:
                    // Repeat the current dialogue node 
                    ProcessNodeType(GetNodeByGuid(_currentDialogueNodeData.NodeGuid));
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogWarning($"Unhandled EndNodeType: {endNodeData.EndNodeType}");
#endif
                    break;
            }
        }

        #endregion End Node
    }
}
