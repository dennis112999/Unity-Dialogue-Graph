using UnityEngine;

using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using Dennis.Tools.DialogueGraph.Data;
using UnityEngine.UIElements;

namespace Dennis.Tools.DialogueGraph
{
    public class BranchNode : BaseNode
    {
        private BranchNodeData _currentNodeData;
        public BranchNodeData CurrentNodeData { get { return _currentNodeData; } }

        public BranchNode() { }

        public BranchNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            _currentNodeData = new BranchNodeData();

            // Initialize node
            title = "Branch Node";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = Guid.NewGuid().ToString();

            // Load the style Sheets
            StyleSheet styleSheet = Resources.Load<StyleSheet>("BranchNodeStyleSheet");
            styleSheets.Add(styleSheet);

            // Set up ports
            AddInputPort("Previous", Port.Capacity.Multi);
            AddOutputPort("True");
            AddOutputPort("False");

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
            ConditionData conditionData = new ConditionData();
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Initialize UI
            OnConditionTypeChanged(conditionData, boxContainer, ConditionType.IsTrue);

            // Add to UI
            mainContainer.Add(boxContainer);
            _currentNodeData.ConditionDatas.Add(conditionData);
        }

        private void RestoreCondition(ConditionData conditionData)
        {
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Generate UI based on existing data
            OnConditionTypeChanged(conditionData, boxContainer, conditionData.ConditionType);

            // Add to UI
            mainContainer.Add(boxContainer);
        }

        private void OnConditionTypeChanged(ConditionData conditionData, Box boxContainer, ConditionType newValue)
        {
            conditionData.ConditionType = newValue;
            boxContainer.Clear();

            // Left side: Text input field
            TextField conditionTextField = UIHelper.CreateTextField(conditionData.ConditionText, newValue =>
            {
                conditionData.ConditionText = newValue;
            }, "TextField");

            // Middle: Enum selection field
            EnumField conditionTypeEnumField = UIHelper.CreateEnumField(conditionData.ConditionType, updatedValue =>
            {
                OnConditionTypeChanged(conditionData, boxContainer, (ConditionType)updatedValue);
            }, "Enum");

            // Right side: Remove button
            Button removeButton = UIHelper.CreateButton("Remove", () =>
            {
                OnRemoveButtonClick(boxContainer, conditionData);
            }, "RemoveButton");

            // Add base UI elements
            boxContainer.Add(conditionTextField);
            boxContainer.Add(conditionTypeEnumField);

            // Add comparison value field (if applicable)
            AddComparisonValueField(conditionData, boxContainer, newValue);

            boxContainer.Add(removeButton);
        }

        private void OnRemoveButtonClick(Box boxContainer, ConditionData conditionData)
        {
            _currentNodeData.ConditionDatas.Remove(conditionData);
            mainContainer.Remove(boxContainer);
        }

        public void Init(BranchNodeData branchNodeData)
        {
            _currentNodeData = branchNodeData;

            // Restore all existing conditions
            foreach (var condition in _currentNodeData.ConditionDatas)
            {
                RestoreCondition(condition);
            }

            // Refresh Node
            RefreshPorts();
            RefreshExpandedState();
        }
    }

}