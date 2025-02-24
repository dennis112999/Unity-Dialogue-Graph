using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class ChoiceNode : BaseNode
    {
        private ChoiceNodeData _currentNodeData;
        public ChoiceNodeData CurrentNodeData => _currentNodeData;

        private Box _choiceStateEnumBox;

        public ChoiceNode() { }

        public ChoiceNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            _currentNodeData = new ChoiceNodeData();

            // Load style sheet
            StyleSheet styleSheet = Resources.Load<StyleSheet>("ChoiceNodeStyleSheet");
            styleSheets.Add(styleSheet);

            // Initialize node
            title = "Choice Node";
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

        #region UI Creation

        private void AddDropdownMenu()
        {
            ToolbarMenu menu = new ToolbarMenu { text = "Add Content" };
            menu.menu.AppendAction("Add Condition", _ => AddCondition());
            titleButtonContainer.Add(menu);
        }

        private void CreateTextFieldBox()
        {
            Box boxContainer = UIHelper.CreateBox("TextLineBox");

            // Input field
            var textField = UIHelper.CreateTextField(_currentNodeData.Text, newValue =>
            {
                _currentNodeData.Text = newValue;
            });
            boxContainer.Add(textField);

            // Audio selection field
            ObjectField audioClipField = UIHelper.CreateObjectField<AudioClip>(
                null, newAudioClip => _currentNodeData.AudioClip = newAudioClip
            );
            boxContainer.Add(audioClipField);

            mainContainer.Add(boxContainer);
        }

        private void CreateChoiceStateEnumBox()
        {
            _choiceStateEnumBox = UIHelper.CreateBox("BoxRow");

            EnumField choiceStateEnumField = UIHelper.CreateEnumField(_currentNodeData.ChoiceNodeFailAction, newValue =>
            {
                _currentNodeData.ChoiceNodeFailAction = newValue;
            });

            Label enumLabel = UIHelper.CreateLabel("If the condition fails", "ChoiceLabel");

            _choiceStateEnumBox.Add(choiceStateEnumField);
            _choiceStateEnumBox.Add(enumLabel);
            mainContainer.Add(_choiceStateEnumBox);
        }

        #endregion

        #region Condition Management

        private void AddCondition()
        {
            ConditionData conditionData = new ConditionData();
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Initialize UI
            OnConditionTypeChanged(conditionData, boxContainer, ConditionType.IsTrue);

            // Add to UI
            mainContainer.Add(boxContainer);
            _currentNodeData.ConditionDatas.Add(conditionData);

            ShowHideChoiceEnum();
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
            ShowHideChoiceEnum();
        }

        #endregion

        #region Initialization

        public void Init(ChoiceNodeData choiceNodeData = null)
        {
            if(choiceNodeData != null)
            {
                _currentNodeData = choiceNodeData;
            }

            // Create Text and Audio UI
            CreateTextFieldBox();

            CreateChoiceStateEnumBox();

            // Restore all existing conditions
            foreach (var condition in _currentNodeData.ConditionDatas)
            {
                RestoreCondition(condition);
            }

            // Ensure the UI reflects the correct state
            ShowHideChoiceEnum();

            // Refresh Node
            RefreshPorts();
            RefreshExpandedState();
        }

        private void ShowHideChoiceEnum()
        {
            _choiceStateEnumBox.style.display = _currentNodeData.ConditionDatas.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #endregion
    }
}
