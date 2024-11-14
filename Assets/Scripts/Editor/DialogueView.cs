using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

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
        }
    }
}