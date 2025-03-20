using Dennis.Tools.DialogueGraph.Data;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using System;
using UnityEditor;

namespace Dennis.Tools.DialogueGraph
{
    public class EventNode : BaseNode
    {
        private EventNodeData _currentNodeData;
        public EventNodeData CurrentNodeData => _currentNodeData;

        public EventNode() { }

        public EventNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            _currentNodeData = new EventNodeData();

            // Load style sheet
            StyleSheet styleSheet = Resources.Load<StyleSheet>("EventNodeStyleSheet");
            styleSheets.Add(styleSheet);

            // Initialize node
            title = "Event Node";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = Guid.NewGuid().ToString();

            // Set up ports
            Port inputPort = AddInputPort("Input", Port.Capacity.Multi);
            inputPort.portColor = Color.yellow;
            AddOutputPort("Output", Port.Capacity.Single);

            // Create UI Components
            AddDropdownMenu();

            // Refresh UI
            base.RefeshUI();
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu menu = new ToolbarMenu { text = "Add Content" };
            menu.menu.AppendAction("Add Event", _ => AddDialogueEvent());
            menu.menu.AppendAction("Add Condition", _ => AddCondition());
            titleButtonContainer.Add(menu);
        }

        #region Dialogue Event

        private void AddDialogueEvent()
        {
            DialogueEventSO dialogueEventSO = null;
            CreateDialogueEventBox(dialogueEventSO, newDialogueEventSO =>
            {
                _currentNodeData.DialogueEventSOs.Add(newDialogueEventSO);
            });
        }

        private void RestoreDialogueEvent(DialogueEventSO dialogueEventSO)
        {
            CreateDialogueEventBox(dialogueEventSO, newDialogueEventSO => dialogueEventSO = newDialogueEventSO);
        }

        private void CreateDialogueEventBox(DialogueEventSO dialogueEventSO, Action<DialogueEventSO> onValueChanged)
        {
            Box boxContainer = UIHelper.CreateBox("DialogueEventSOContainer");

            // DialogueEventSO selection field
            ObjectField dialogueEventSOField = UIHelper.CreateObjectField<DialogueEventSO>(
                dialogueEventSO, newDialogueEventSO =>
                {
#if UNITY_EDITOR
                    if (newDialogueEventSO != null && !AssetDatabase.Contains(newDialogueEventSO))
                    {
                        Debug.LogWarning($"The selected object '{newDialogueEventSO.name}' is not an asset and has been ignored.");
                        return;
                    }
#endif
                    onValueChanged(newDialogueEventSO);
                },
                "ObjectField"
            );
            boxContainer.Add(dialogueEventSOField);

            // Right side: Remove button
            Button removeButton = UIHelper.CreateButton("Remove", () =>
            {
                OnRemoveButtonClick(boxContainer, dialogueEventSO);
            }, "RemoveButton");

            boxContainer.Add(removeButton);

            // Add to UI
            mainContainer.Add(boxContainer);
        }

        private void OnRemoveButtonClick(Box boxContainer, DialogueEventSO dialogueEventSO)
        {
            _currentNodeData.DialogueEventSOs.Remove(dialogueEventSO);
            mainContainer.Remove(boxContainer);
        }

        #endregion Dialogue Event

        #region Conditions

        private void AddCondition()
        {
            VariableOperationData conditionData = new VariableOperationData();
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Initialize UI
            OnConditionTypeChanged(conditionData, boxContainer, VariableOperationType.SetTrue);

            // Add to UI
            mainContainer.Add(boxContainer);
            _currentNodeData.VariableOperationDatas.Add(conditionData);
        }

        private void RestoreCondition(VariableOperationData operationData)
        {
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Generate UI based on existing data
            OnConditionTypeChanged(operationData, boxContainer, operationData.OperationType);

            // Add to UI
            mainContainer.Add(boxContainer);
        }

        private void OnConditionTypeChanged(VariableOperationData variableOperationData, Box boxContainer, VariableOperationType newValue)
        {
            variableOperationData.OperationType = newValue;
            boxContainer.Clear();

            // Left side: Text input field
            TextField conditionTextField = UIHelper.CreateTextField(variableOperationData.VariableName, newValue =>
            {
                variableOperationData.VariableName = newValue;
            }, "TextField");

            // Middle: Enum selection field
            EnumField conditionTypeEnumField = UIHelper.CreateEnumField(variableOperationData.OperationType, updatedValue =>
            {
                OnConditionTypeChanged(variableOperationData, boxContainer, updatedValue);
            }, "Enum");

            // Right side: Remove button
            Button removeButton = UIHelper.CreateButton("Remove", () =>
            {
                OnRemoveButtonClick(boxContainer, variableOperationData);
            }, "RemoveButton");

            // Add base UI elements
            boxContainer.Add(conditionTextField);
            boxContainer.Add(conditionTypeEnumField);

            // Add comparison value field (if applicable)
            AddValueField(variableOperationData, boxContainer, newValue);

            boxContainer.Add(removeButton);
        }

        private void OnRemoveButtonClick(Box boxContainer, VariableOperationData variableOperationData)
        {
            _currentNodeData.VariableOperationDatas.Remove(variableOperationData);
            mainContainer.Remove(boxContainer);
        }

        private void AddValueField(VariableOperationData conditionData, Box boxContainer, VariableOperationType variableOperationType)
        {
            switch (variableOperationType)
            {
                case VariableOperationType.Add:
                case VariableOperationType.Subtract:
                case VariableOperationType.Multiply:
                case VariableOperationType.Divide:
                    FloatField valueField = UIHelper.CreateFloatField(conditionData.Value, newValue =>
                    {
                        conditionData.Value = newValue;
                    });

                    boxContainer.Add(valueField);
                    break;

                default:
                case VariableOperationType.SetTrue:
                case VariableOperationType.SetFalse:
                    // No additional input field needed
                    break;
            }
        }

        #endregion Conditions

        public void Init(EventNodeData eventNodeData)
        {
            _currentNodeData = eventNodeData;

            // Restore all existing conditions
            foreach (var condition in _currentNodeData.VariableOperationDatas)
            {
                RestoreCondition(condition);
            }

            // Restore all dialogue Events
            foreach (var dialogueEventSO in _currentNodeData.DialogueEventSOs)
            {
                RestoreDialogueEvent(dialogueEventSO);
            }

            // Refresh Node
            base.RefeshUI();
        }
    }

}