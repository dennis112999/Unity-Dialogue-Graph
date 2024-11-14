using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

        private NodeSearchWindow _searchWindow;

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

            AddElement(GenerateEntryPointNode());
            AddSearchWindow(editorWindow);
        }

        private void AddSearchWindow(EditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        #region Node

        /// <summary>
        /// Create Node
        /// </summary>
        /// <param name="nodeName"></param>
        public void CreateNode(string nodeName, Vector2 pos)
        {
            AddElement(CreateDialogueNode(nodeName, pos));
        }

        /// <summary>
        /// Generate Entry Point Node at init
        /// </summary>
        /// <returns>Point Node</returns>
        private Node GenerateEntryPointNode()
        {
            var node = new DialogueNode
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntryPoint = true
            };

            // Add Port
            var generatePort = GeneratePort(node, Direction.Output);
            generatePort.portName = "Next";
            node.outputContainer.Add(generatePort);

            // Set node capabilities to Unmovable and Undeletable
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        /// <summary>
        /// Create Dialogue Node
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="nodePos"></param>
        /// <returns></returns>
        private DialogueNode CreateDialogueNode(string nodeName, Vector2 nodePos = default(Vector2))
        {
            var dialogueNode = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);

            // Load the style Sheets
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Set dialogue text field
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                dialogueNode.DialogueText = evt.newValue;
                dialogueNode.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(dialogueNode.title);
            dialogueNode.mainContainer.Add(textField);

            // Refresh Node
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(nodePos, defaultNodeSize));

            return dialogueNode;
        }

        #endregion Node

        #region Port

        private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
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
    }
}