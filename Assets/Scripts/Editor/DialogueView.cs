using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using static UnityEditor.Experimental.GraphView.Port;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueView : GraphView
    {
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
        private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
    }
}