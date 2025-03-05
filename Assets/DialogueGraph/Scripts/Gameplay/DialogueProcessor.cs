using Dennis.Tools.DialogueGraph.Data;
using System.Collections.Generic;
using UnityEngine;
using Dennis.Tools.DialogueGraph.UI;

using Dennis.Tools.DialogueGraph.Event;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueProcessor : DialogueFlowHandler
    {
        [SerializeField] private DialogueControllerUI _dialogueControllerUI;

        private List<DialogueElementBase> _baseContainers;
        private int _currentIndex = 0;

        public void StartDialogue()
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

                    break;

                case EventNodeData eventNodeData:
                    ExecuteEventNode(eventNodeData);
                    break;

                case BranchNodeData branchNodeData:
                    ExecuteBranchNode(branchNodeData);
                    break;
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

            // Determine the next event node and process it
            ProcessNodeType(GetNextNode(eventNodeData));
        }

        #endregion Event Node

        #region Branch Node

        private void ExecuteBranchNode(BranchNodeData branchNodeData)
        {
            // Iterate through all conditions in the branch node
            foreach (ConditionData conditionData in branchNodeData.ConditionDatas)
            {
                // If any condition is NOT met, execute the False branch and return early
                if (!VariableManager.EvaluateCondition(conditionData))
                {
                    ProcessNodeType(GetNodeByGuid(branchNodeData.FalseGuidNode));
                    return;
                }
            }

            // If all conditions are met, execute the True branch
            ProcessNodeType(GetNodeByGuid(branchNodeData.TrueGuidNode));
        }

        #endregion Branch Node

        #region Dialogue Node

        {
            _currentIndex = 0;

            _baseContainers.Sort(delegate (DialogueElementBase x, DialogueElementBase y)
            {
                return x.OrderIndex.CompareTo(y.OrderIndex);
            });

            ProcessDialogue();
        }

        private void ProcessDialogue()
        {
            for (int i = _currentIndex; i < _baseContainers.Count; i++)
            {
                _currentIndex = i + 1;
                if (_baseContainers[i] is DialogueImagesData)
                {
                    DialogueImagesData tmp = _baseContainers[i] as DialogueImagesData;
                    _dialogueControllerUI.SetImage(tmp.Sprite_Left, tmp.Sprite_Right);
                }
                else if (_baseContainers[i] is DialogueBoxData)
                {
                    DialogueBoxData tmp = _baseContainers[i] as DialogueBoxData;
                    _dialogueControllerUI.SetContentText(tmp.Text);

                    // TODO - Audio
                    break;
                }
            }
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
