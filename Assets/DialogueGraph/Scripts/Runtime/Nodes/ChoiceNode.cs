using UnityEngine;
using Dennis.Tools.DialogueGraph.Data;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace Dennis.Tools.DialogueGraph
{
    public class ChoiceNode : BaseConditionNode<ChoiceNodeData>
    {
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
            guid = System.Guid.NewGuid().ToString();

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
            menu.menu.AppendAction("Add Condition", _ => AddCondition());
            titleButtonContainer.Add(menu);
        }

        public void Init(ChoiceNodeData choiceNodeData = null)
        {
            if (choiceNodeData != null)
            {
                _currentNodeData = choiceNodeData;
            }

            CreateChoiceStateEnumBox();

            // Restore all existing conditions
            foreach (var condition in _currentNodeData.ConditionDatas)
            {
                RestoreCondition(condition);
            }

            ShowHideChoiceEnum();

            // Refresh Node
            base.RefeshUI();
        }

        protected override void ShowHideChoiceEnum()
        {
            _choiceStateEnumBox.style.display = _currentNodeData.ConditionDatas.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
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
    }
}
