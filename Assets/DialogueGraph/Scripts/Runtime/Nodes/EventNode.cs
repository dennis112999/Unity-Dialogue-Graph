using Dennis.Tools.DialogueGraph.Data;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using System;

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
            StyleSheet styleSheet = Resources.Load<StyleSheet>("ChoiceNodeStyleSheet");
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
            RefreshExpandedState();
            RefreshPorts();
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu menu = new ToolbarMenu { text = "Add Content" };
            menu.menu.AppendAction("Add Condition", _ => AddCondition());
            titleButtonContainer.Add(menu);
        }

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

        public void Init(EventNodeData eventNodeData)
        {
            _currentNodeData = eventNodeData;

            // Restore all existing conditions
            foreach (var condition in _currentNodeData.VariableOperationDatas)
            {
                RestoreCondition(condition);
            }

            // Refresh Node
            RefreshPorts();
            RefreshExpandedState();
        }
    }

}