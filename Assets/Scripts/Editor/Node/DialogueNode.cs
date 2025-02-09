using UnityEngine.UIElements;
using UnityEngine;

using System;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueNode : BaseNode
    {
        private DialogueNodeData _currentNodeData;
        public DialogueNodeData CurrentNodeData { get { return _currentNodeData; } }

        public DialogueNode() { }

        public DialogueNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            // Init NodeData
            _currentNodeData = new DialogueNodeData();

            // Load the style Sheets
            StyleSheet styleSheet = Resources.Load<StyleSheet>("DialogueNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Dialogue Node";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = Guid.NewGuid().ToString();

            AddInputPort("Previous", Port.Capacity.Multi);
            AddOutputPort("Next");

            AddChoiceButton();
            AddDropdownMenu();

            // Refresh
            RefreshPorts();
            RefreshExpandedState();
        }

        private void AddChoiceButton()
        {
            // Create ChoiceButton
            Button addChoiceButton = UIHelper.CreateButton("Add Choice", () =>
            {
                AddChoicePort(this);
            }, "TopBtn");

            titleButtonContainer.Add(addChoiceButton);
        }


        public Port AddChoicePort(BaseNode baseNode)
        {
            Port port = GetPortInstance(Direction.Output);

            // Delete button
            Button deleteButton = UIHelper.CreateButton("X", () => DeletePort(baseNode, port));
            port.contentContainer.Add(deleteButton);

            // Add Choice Port
            port.portName = "Next";
            Label portNameLabel = port.contentContainer.Q<Label>("type"); 
            portNameLabel.AddToClassList("PortName");

            // Set color of the port
            port.portColor = Color.yellow;

            baseNode.outputContainer.Add(port);

            baseNode.RefreshPorts();
            baseNode.RefreshExpandedState();

            return port;
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Content";

            Menu.menu.AppendAction("Text", new Action<DropdownMenuAction>(x => AddNewDialogueBox()));
            Menu.menu.AppendAction("Image", new Action<DropdownMenuAction>(x => AddNewDialogueBox()));

            titleButtonContainer.Add(Menu);
        }

        private void DeletePort(BaseNode node, Port port)
        {
            IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

            if (portEdge.Any())
            {
                Edge edge = portEdge.First();
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                graphView.RemoveElement(edge);
            }

            node.outputContainer.Remove(port);

            // Refresh
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        #region Dialogue Box

        private void AddNewDialogueBox()
        {
            // Create a new DialogueBoxData
            var dialogueBox = new DialogueBoxData();
            _currentNodeData.DialogueBoxes.Add(dialogueBox);

            AddDialogueBox(dialogueBox);
        }

        private void AddDialogueBox(DialogueBoxData dialogueBox)
        {
            // Create a container box
            Box boxContainer = UIHelper.CreateBox("TopBox");

            // Add a label
            Label label = UIHelper.CreateLabel("Dialogue", "LabelText");
            boxContainer.Add(label);

            // Set dialogue text field
            var textField = UIHelper.CreateTextField(dialogueBox.Text, newValue =>
            {
                dialogueBox.Text = newValue;
            });

            // Create a remove button
            Button btnRemove = UIHelper.CreateButton("Remove", () =>
            {
                _currentNodeData.DialogueBoxes.Remove(dialogueBox);
                mainContainer.Remove(boxContainer); // Remove the entire container
                mainContainer.Remove(textField);
            }, "TextRemoveBtn");
            boxContainer.Add(btnRemove);

            // Add the container to the main container
            mainContainer.Add(boxContainer);
            mainContainer.Add(textField);
        }

        #region Init

        public void Init(DialogueNodeData dialogueNodeData)
        {
            _currentNodeData = dialogueNodeData;

            foreach (var dialogueBox in _currentNodeData.DialogueBoxes)
            {
                AddDialogueBox(dialogueBox);
            }

            // Refresh
            RefreshPorts();
            RefreshExpandedState();
        }
        
        #endregion Init

    }
}
