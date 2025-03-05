using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

        private NodeSearchWindow _searchWindow;
        private DialogueGraphWindow _dialogueGraphWindow;

        private bool _hasStartNode = false;

        public DialogueView(EditorWindow editorWindow)
        {
            // Load the style Sheets
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

            // SetUp Zoom
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Add Manipulator - selectionDragger...etc.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Set up background
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow(editorWindow);

            _dialogueGraphWindow = editorWindow as DialogueGraphWindow;
        }

        private void AddSearchWindow(EditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public void ResetGraph()
        {
            _hasStartNode = false;
        }

        #region Port

        /// <summary>
        /// Connect the ports
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        #endregion Port

        #region NodeType

        public bool CreateStartNode(Vector2 position)
        {
            if(_hasStartNode) return false;

            _hasStartNode = true;
            AddElement(new StartNode(position, _dialogueGraphWindow, this));

            return true;
        }

        public void CreateStartNode(Vector2 position, string GUID)
        {
            if (_hasStartNode)
            {
#if UNITY_EDITOR
                Debug.LogWarning("A StartNode already exists in the graph. Skipping creation.");
#endif
            }

            _hasStartNode = true;

            StartNode tempNode = new StartNode(position, _dialogueGraphWindow, this);
            tempNode.GUID = GUID;

            AddElement(tempNode);
        }

        public void CreateEndNode(Vector2 position)
        {
            EndNode tempNode = new EndNode(position, _dialogueGraphWindow, this);
            tempNode.Init();

            AddElement(tempNode);
        }

        public void CreateEndNode(Vector2 position, EndNodeData endNodeData)
        {
            EndNode tempNode = new EndNode(position, _dialogueGraphWindow, this);
            tempNode.GUID = endNodeData.NodeGuid;
            tempNode.Init(endNodeData);

            AddElement(tempNode);
        }

        public void CreateDialogueNode(Vector2 position)
        {
            AddElement(new DialogueNode(position, _dialogueGraphWindow, this));
        }

        public void CreateDialogueNode(Vector2 position, DialogueNodeData dialogueNodeData)
        {
            DialogueNode tempNode = new DialogueNode(position, _dialogueGraphWindow, this);
            tempNode.GUID = dialogueNodeData.NodeGuid;
            tempNode.Init(dialogueNodeData);

            AddElement(tempNode);
        }

        public void CreateChoiceNode(Vector2 position)
        {
            ChoiceNode tempNode = new ChoiceNode(position, _dialogueGraphWindow, this);
            tempNode.Init();

            AddElement(tempNode);
        }

        public void CreateChoiceNode(Vector2 position, ChoiceNodeData choiceNodeData)
        {
            ChoiceNode tempNode = new ChoiceNode(position, _dialogueGraphWindow, this);
            tempNode.GUID = choiceNodeData.NodeGuid;
            tempNode.Init(choiceNodeData);

            AddElement(tempNode);
        }

        public void CreateBranchNode(Vector2 position)
        {
            BranchNode tempNode = new BranchNode(position, _dialogueGraphWindow, this);
            AddElement(tempNode);
        }

        public void CreateBranchNode(Vector2 position, BranchNodeData branchNodeData)
        {
            BranchNode tempNode = new BranchNode(position, _dialogueGraphWindow, this);
            tempNode.GUID = branchNodeData.NodeGuid;
            tempNode.Init(branchNodeData);

            AddElement(tempNode);
        }

        public void CreateEventNode(Vector2 position)
        {
            EventNode tempNode = new EventNode(position, _dialogueGraphWindow, this);
            AddElement(tempNode);
        }

        public void CreateEventNode(Vector2 position, EventNodeData eventNodeData)
        {
            EventNode tempNode = new EventNode(position, _dialogueGraphWindow, this);
            tempNode.GUID = eventNodeData.NodeGuid;
            tempNode.Init(eventNodeData);

            AddElement(tempNode);
        }

        #endregion NodeType
    }
}