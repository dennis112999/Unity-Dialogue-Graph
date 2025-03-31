#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Dennis.Tools.DialogueGraph.Data;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Dennis.Tools.DialogueGraph
{
    public class BranchNode : BaseConditionNode<BranchNodeData>
    {
        public BranchNodeData CurrentNodeData => _currentNodeData;

        public BranchNode() { }

        public BranchNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            _currentNodeData = new BranchNodeData();

            // Initialize node
            title = "Branch Node";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = System.Guid.NewGuid().ToString();

            // Load style Sheets
            StyleSheet styleSheet = Resources.Load<StyleSheet>("BranchNodeStyleSheet");
            styleSheets.Add(styleSheet);

            // Set up ports
            AddInputPort("Previous", Port.Capacity.Multi);
            AddOutputPort("True");
            AddOutputPort("False");

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

        public void Init(BranchNodeData branchNodeData)
        {
            _currentNodeData = branchNodeData;

            // Restore all existing conditions
            foreach (var condition in _currentNodeData.ConditionDatas)
            {
                RestoreCondition(condition);
            }

            // Refresh Node
            base.RefeshUI();
        }
    }
}

#endif