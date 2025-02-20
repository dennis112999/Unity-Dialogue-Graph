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

        private List<VisualElement> _dialogueElements = new List<VisualElement>();

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

        #region Choice Management

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

        #endregion Choice Management

        #region Dialogue Box

        private void AddNewDialogueBox()
        {
            // Create a new DialogueBoxData
            DialogueBoxData dialogueBox = new DialogueBoxData();
            _currentNodeData.DialogueBoxes.Add(dialogueBox);
            _currentNodeData.AllDialogueElements.Add(dialogueBox);

            AddDialogueBox(dialogueBox);
        }
        private void AddDialogueBox(DialogueBoxData dialogueBox)
        {
            Box boxContainer = UIHelper.CreateBox("DialogueBox");
            boxContainer.AddToClassList("DialogueBox");

            // Create the title section
            Box titleContainer = UIHelper.CreateBox("DialogueBoxTitle");

            // Title label
            Label label = UIHelper.CreateLabel("Dialogue Text", "LabelText");
            titleContainer.Add(label);

            // Add "Move Up", "Move Down", and "Remove" buttons
            titleContainer.Add(CreateMoveAndRemoveButtons(dialogueBox));

            boxContainer.Add(titleContainer);

            // Background container
            Box contentBox = UIHelper.CreateBox("DialogueContentBox");

            // Input field
            var textField = UIHelper.CreateTextField(dialogueBox.Text, newValue =>
            {
                dialogueBox.Text = newValue;
            });
            textField.AddToClassList("TextField");
            contentBox.Add(textField);

            // Audio selection field
            ObjectField audioClipField = UIHelper.CreateObjectField<AudioClip>(
                null,
                (newAudioClip) => dialogueBox.AudioClip = newAudioClip
            );
            audioClipField.AddToClassList("AudioClipField");
            contentBox.Add(audioClipField);

            boxContainer.Add(contentBox);
            mainContainer.Add(boxContainer);
            _dialogueElements.Add(boxContainer);
        }

        #endregion Dialogue Box

        #region Dialogue Image

        private void CreateAndAddDialogueImage()
        {
            // Create a new DialogueImageData
            DialogueImagesData dialogueImages = new DialogueImagesData();
            _currentNodeData.DialogueImagesDatas.Add(dialogueImages);
            _currentNodeData.AllDialogueElements.Add(dialogueImages);

            AddDialogueImageContainer(dialogueImages);
        }

        public void AddDialogueImageContainer(DialogueImagesData imagesData)
        {
            Box boxContainer = UIHelper.CreateBox("DialogueBox");
            boxContainer.AddToClassList("DialogueBox");

            // Create the title section
            Box titleContainer = UIHelper.CreateBox("DialogueBoxTitle");

            // Title label
            Label label = UIHelper.CreateLabel("Image", "LabelText");
            titleContainer.Add(label);

            // Add "Move Up", "Move Down", and "Remove" buttons
            titleContainer.Add(CreateMoveAndRemoveButtons(imagesData));

            boxContainer.Add(titleContainer);

            // Create the image preview section
            CreateDialogueImagePreview(imagesData, boxContainer);

            // Add the container to the main UI
            mainContainer.Add(boxContainer);
            _dialogueElements.Add(boxContainer);
        }

        private void CreateDialogueImagePreview(DialogueImagesData imagesData, Box boxContainer)
        {
            // Set up the Image Preview Box
            Box imagePreviewBox = UIHelper.CreateBox("ImagePreviewBox");
            Box imagesBox = UIHelper.CreateBox("ImagesBox");

            // Left and right image previews
            Image leftImage = UIHelper.CreateImage("ImagePreview");
            leftImage.AddToClassList("ImagePreviewLeft");
            imagePreviewBox.Add(leftImage);

            Image rightImage = UIHelper.CreateImage("ImagePreview");
            rightImage.AddToClassList("ImagePreviewRight");
            imagePreviewBox.Add(rightImage);

            // Sprite selection fields
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

            objectField_Left.AddToClassList("SpriteLeft");
            objectField_Right.AddToClassList("SpriteRight");

            imagesBox.Add(objectField_Left);
            imagesBox.Add(objectField_Right);

            boxContainer.Add(imagePreviewBox);
            boxContainer.Add(imagesBox);
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

            var elementsCopy = _currentNodeData.AllDialogueElements.ToList();
            foreach (var element in elementsCopy)
            {
                AddDialogueElement(element);
            }

            // Refresh
            RefreshPorts();
            RefreshExpandedState();
        }

        #endregion Init

        private Box CreateMoveAndRemoveButtons(DialogueElementBase element)
        {
            Box buttonContainer = UIHelper.CreateBox("ButtonGroup");

            // Move Up Button
            Button btnUp = UIHelper.CreateButton("▲", () =>
            {
                MoveElement(element, -1);
            }, "MoveUpBtn");
            buttonContainer.Add(btnUp);

            // Move Down Button
            Button btnDown = UIHelper.CreateButton("▼", () =>
            {
                MoveElement(element, 1);
            }, "MoveDownBtn");
            buttonContainer.Add(btnDown);

            // Remove Button
            Button btnRemove = UIHelper.CreateButton("Remove", () =>
            {
                RemoveElement(element);
            }, "TextRemoveBtn");
            buttonContainer.Add(btnRemove);

            return buttonContainer;
        }

        #region Element Management

        private void AddDialogueElement(DialogueElementBase element)
        {
            switch (element)
            {
                case DialogueImagesData imageData:
                    AddDialogueImageContainer(imageData);
                    break;
                case DialogueBoxData dialogueData:
                    AddDialogueBox(dialogueData);
                    break;
            }
        }

        private void MoveElement(DialogueElementBase element, int direction)
        {
            int index = _currentNodeData.AllDialogueElements.IndexOf(element);
            int newIndex = index + direction;

            if (newIndex < 0 || newIndex >= _currentNodeData.AllDialogueElements.Count)
                return;

            (_currentNodeData.AllDialogueElements[index], _currentNodeData.AllDialogueElements[newIndex]) =
                (_currentNodeData.AllDialogueElements[newIndex], _currentNodeData.AllDialogueElements[index]);

            for (int i = 0; i < _currentNodeData.AllDialogueElements.Count; i++)
            {
                _currentNodeData.AllDialogueElements[i].OrderIndex = i;
            }

            RefreshUI();
        }

        private void RemoveElement(DialogueElementBase element)
        {
            if (!_currentNodeData.AllDialogueElements.Contains(element)) return;

            // Remove from AllDialogueElements
            _currentNodeData.AllDialogueElements.Remove(element);

            // Also remove from specific lists
            switch (element)
            {
                case DialogueBoxData dialogueBox:
                    _currentNodeData.DialogueBoxes.Remove(dialogueBox);
                    break;

                case DialogueImagesData dialogueImage:
                    _currentNodeData.DialogueImagesDatas.Remove(dialogueImage);
                    break;
            }

            // Reassign OrderIndex
            for (int i = 0; i < _currentNodeData.AllDialogueElements.Count; i++)
            {
                _currentNodeData.AllDialogueElements[i].OrderIndex = i;
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            var elementsToRemove = _dialogueElements.ToList();

            foreach (var element in elementsToRemove)
            {
                if (mainContainer.Contains(element))
                {
                    mainContainer.Remove(element);
                }
            }

            _dialogueElements.Clear();

            foreach (var element in _currentNodeData.AllDialogueElements)
            {
                AddDialogueElement(element);
            }
        }

        #endregion Element Management
    }
}
