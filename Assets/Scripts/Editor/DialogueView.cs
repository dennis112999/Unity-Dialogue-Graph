using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

        private NodeSearchWindow _searchWindow;
        private DialogueGraphWindow _dialogueGraphWindow;

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

        public void CreateStartNode(Vector2 position)
        {
            AddElement(new StartNode(position, _dialogueGraphWindow, this));
        }

        public void CreateEndNode(Vector2 position)
        {
            AddElement(new EndNode(position, _dialogueGraphWindow, this));
        }

        public void CreateDialogueNode(Vector2 position)
        {
            AddElement(new DialogueNode(position, _dialogueGraphWindow, this));
        }

        #endregion NodeType
    }
}