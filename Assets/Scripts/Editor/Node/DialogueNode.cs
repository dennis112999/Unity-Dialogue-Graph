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
            Menu.menu.AppendAction("Image", new Action<DropdownMenuAction>(x => CreateAndAddDialogueImage()));

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
            DialogueBoxData dialogueBox = new DialogueBoxData();
            _currentNodeData.DialogueBoxes.Add(dialogueBox);

            AddDialogueBox(dialogueBox);
        }

        private void AddDialogueBox(DialogueBoxData dialogueBox)
        {
            // Create a imagesData box
            Box boxContainer = UIHelper.CreateBox("TopBox");

            // Add a label
            Label label = UIHelper.CreateLabel("Dialogue", "LabelText");
            boxContainer.Add(label);

            // Set dialogue text field
            var textField = UIHelper.CreateTextField(dialogueBox.Text, newValue =>
            {
                dialogueBox.Text = newValue;
            });

            // Create AudioClip selection field
            ObjectField audioClipField = UIHelper.CreateObjectField<AudioClip>(
                null,
                (newAudioClip) =>
                {
                    dialogueBox.AudioClip = newAudioClip;
                }
            );

            // Create a remove button
            Button btnRemove = UIHelper.CreateButton("Remove", () =>
            {
                _currentNodeData.DialogueBoxes.Remove(dialogueBox);
                mainContainer.Remove(boxContainer); // Remove the entire imagesData
                mainContainer.Remove(textField);
                mainContainer.Remove(audioClipField);
            }, "TextRemoveBtn");
            boxContainer.Add(btnRemove);

            // Add the imagesData to the main imagesData
            mainContainer.Add(boxContainer);
            mainContainer.Add(textField);
            mainContainer.Add(audioClipField);
        }

        #endregion Dialogue Box

        #region Dialogue Image

        private void CreateAndAddDialogueImage()
        {
            // Create a new DialogueImageData
            DialogueImagesData dialogueImages = new DialogueImagesData();
            _currentNodeData.DialogueImagesDatas.Add(dialogueImages);

            AddDialogueImageContainer(dialogueImages);
        }

        public void AddDialogueImageContainer(DialogueImagesData ImagesData)
        {
            Box boxContainer = UIHelper.CreateBox("DialogueBox");

            // Add a label
            Label label = UIHelper.CreateLabel("Image", "LabelText");
            boxContainer.Add(label);

            // Create a remove button
            Button btnRemove = UIHelper.CreateButton("Remove", () =>
            {
                _currentNodeData.DialogueImagesDatas.Remove(ImagesData);
                mainContainer.Remove(boxContainer); // Remove the entire imagesData
            }, "TextRemoveBtn");

            boxContainer.Add(btnRemove);

            CreateDialogueImagePreview(ImagesData, boxContainer);

            mainContainer.Add(boxContainer);
        }

        private void CreateDialogueImagePreview(DialogueImagesData imagesData, Box boxContainer)
        {
            // Set Up Image Box
            Box ImagePreviewBox = UIHelper.CreateBox("BoxRow");
            Box ImagesBox = UIHelper.CreateBox("BoxRow");

            // Set up Image Preview.
            Image leftImage = UIHelper.CreateImage("ImagePreview");
            leftImage.AddToClassList("ImagePreviewLeft");
            ImagePreviewBox.Add(leftImage);

            Image rightImage = UIHelper.CreateImage("ImagePreview");
            rightImage.AddToClassList("ImagePreviewRight");
            ImagePreviewBox.Add(rightImage);

            // Set up Sprite
            ObjectField objectField_Left = GetNewObjectFieldForSprite(
                () => imagesData.Sprite_Left,
                (newSprite) => imagesData.Sprite_Left = newSprite,
                leftImage,
                "SpriteLeft"
            );

            ObjectField objectField_Right = GetNewObjectFieldForSprite(
                () => imagesData.Sprite_Right,
                (newSprite) => imagesData.Sprite_Right = newSprite,
                rightImage,
                "SpriteRight"
            );

            ImagesBox.Add(objectField_Left);
            ImagesBox.Add(objectField_Right);

            // Add to box imagesData
            boxContainer.Add(ImagePreviewBox);
            boxContainer.Add(ImagesBox);
        }

        private ObjectField GetNewObjectFieldForSprite(Func<Sprite> getCurrentSprite, Action<Sprite> updateSprite, Image previewImage, string primaryStyle = "", string additionalStyle = "")
        {
            ObjectField objectField = UIHelper.CreateObjectField<Sprite>(
                getCurrentSprite(),
                (newSprite) =>
                {
                    updateSprite(newSprite);
                    previewImage.image = (newSprite != null ? newSprite.texture : null);
                },
                primaryStyle
            );

            // Set initial image preview
            previewImage.image = (getCurrentSprite() != null ? getCurrentSprite().texture : null);

            // Set USS class for styling
            objectField.AddToClassList(additionalStyle);

            return objectField;
        }

        #endregion Dialogue Image

        #region Init

        public void Init(DialogueNodeData dialogueNodeData)
        {
            _currentNodeData = dialogueNodeData;

            foreach (var dialogueBox in _currentNodeData.DialogueBoxes)
            {
                AddDialogueBox(dialogueBox);
            }

            foreach (var imagesData in _currentNodeData.DialogueImagesDatas)
            {
                AddDialogueImageContainer(imagesData);
            }

            // Refresh
            RefreshPorts();
            RefreshExpandedState();
        }
        
        #endregion Init

    }
}
