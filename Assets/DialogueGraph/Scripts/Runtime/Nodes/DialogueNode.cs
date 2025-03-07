﻿using UnityEngine.UIElements;
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
            base.RefeshUI();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseNode"></param>
        /// <returns></returns>
        public Port AddChoicePort(BaseNode baseNode)
        {
            // Create a new dialogue data port
            DialogueDataPort newDialoguePort = new DialogueDataPort
            {
                PortGuid = Guid.NewGuid().ToString()
            };

            // Call the method to create and configure the port
            return CreateAndConfigurePort(baseNode, newDialoguePort, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="dialogueDataPort"></param>
        /// <returns></returns>
        public Port SetChoicePort(BaseNode baseNode, DialogueDataPort dialogueDataPort)
        {
            // Ensure the existing data is passed
            if (dialogueDataPort == null)
            {
                Debug.LogWarning("SetChoicePort called with null DialogueDataPort.");
                return null;
            }

            return CreateAndConfigurePort(baseNode, dialogueDataPort, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="dialogueDataPort"></param>
        /// <param name="isNew"></param>
        /// <returns></returns>
        private Port CreateAndConfigurePort(BaseNode baseNode, DialogueDataPort dialogueDataPort, bool isNew)
        {
            Port port = GetPortInstance(Direction.Output);

            // Delete button
            Button deleteButton = UIHelper.CreateButton("Remove", () =>
            {
                DeletePort(baseNode, port);
            }, "Remove");
            port.contentContainer.Add(deleteButton);

            // Configure the port
            port.portName = dialogueDataPort.PortGuid;
            Label portNameLabel = port.contentContainer.Q<Label>("type");
            portNameLabel.AddToClassList("PortName");
            port.portColor = Color.yellow;

            // Add to data if new and only add to outputContainer if it's a new port
            if (isNew) _currentNodeData.DialogueDataPorts.Add(dialogueDataPort);

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
            Menu.menu.AppendAction("Name", new Action<DropdownMenuAction>(x => CreateAndAddDialogueName()));

            titleButtonContainer.Add(Menu);
        }

        private void DeletePort(BaseNode node, Port port)
        {
            // Remove Data
            DialogueDataPort tmp = CurrentNodeData.DialogueDataPorts.Find(findPort => findPort.PortGuid == port.portName);
            CurrentNodeData.DialogueDataPorts.Remove(tmp);

            // Remove Port
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
            _currentNodeData.AllDialogueElements.Add(dialogueBox);

            AddDialogueBox(dialogueBox);
        }

        private void AddDialogueBox(DialogueBoxData dialogueBox)
        {
            Box boxContainer = CreateDialogueSection("Dialogue Text", dialogueBox);

            // Background container
            Box contentBox = UIHelper.CreateBox("DialogueContentBox");

            // Background container
            Box textBox = UIHelper.CreateBox("NameRow");

            // Name label (left side)
            Label nameLabel = UIHelper.CreateLabel("Content:", "LabelText");
            textBox.Add(nameLabel);

            // Input field
            var textField = UIHelper.CreateTextField(dialogueBox.Text, newValue =>
            {
                dialogueBox.Text = newValue;
            }, "TextField");
            textBox.Add(textField);

            Box audioBox = UIHelper.CreateBox("NameRow");

            // Name label (left side)
            Label audioLabel = UIHelper.CreateLabel("Audio:", "LabelText");
            audioBox.Add(audioLabel);

            // Audio selection field
            ObjectField audioClipField = UIHelper.CreateObjectField<AudioClip>(
                null,
                (newAudioClip) => dialogueBox.AudioClip = newAudioClip
            , "AudioClipField");
            audioBox.Add(audioClipField);

            contentBox.Add(textBox);
            contentBox.Add(audioBox);

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
            _currentNodeData.AllDialogueElements.Add(dialogueImages);

            AddDialogueImageContainer(dialogueImages);
        }

        public void AddDialogueImageContainer(DialogueImagesData imagesData)
        {
            Box boxContainer = CreateDialogueSection("Images", imagesData);

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

        #region Dialogue Name

        private void CreateAndAddDialogueName()
        {
            // Create a new DialogueNameData
            DialogueNameData dialogueName = new DialogueNameData();
            _currentNodeData.AllDialogueElements.Add(dialogueName);

            AddDialogueName(dialogueName);
        }

        private void AddDialogueName(DialogueNameData dialogueName)
        {
            Box boxContainer = CreateDialogueSection("Name", dialogueName);

            // Background container
            Box contentBox = UIHelper.CreateBox("NameRow");

            // Name label (left side)
            Label nameLabel = UIHelper.CreateLabel("Name:", "LabelText");
            contentBox.Add(nameLabel);

            // Input field (right side)
            var textField = UIHelper.CreateTextField(dialogueName.Name, newValue =>
            {
                dialogueName.Name = newValue;
            }, "TextField");
            contentBox.Add(textField);

            boxContainer.Add(contentBox);

            // Add the container to the main UI
            mainContainer.Add(boxContainer);
            _dialogueElements.Add(boxContainer);
        }

        #endregion Dialogue Name

        #region Init

        public void Init(DialogueNodeData dialogueNodeData)
        {
            _currentNodeData = dialogueNodeData;

            // 
            var elementsCopy = _currentNodeData.AllDialogueElements.ToList();
            foreach (var element in elementsCopy)
            {
                AddDialogueElement(element);
            }

            // 
            foreach (DialogueDataPort port in _currentNodeData.DialogueDataPorts)
            {
                SetChoicePort(this, port);
            }

            // Refresh
            base.RefeshUI();
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

        /// <summary>
        /// Creates a dialogue box UI section, including a title and control buttons
        /// </summary>
        /// <param name="titleText">The text displayed as the title</param>
        /// <param name="element">DialogueElementBase</param>
        /// <returns>dialogue box container</returns>
        private Box CreateDialogueSection(string titleText, DialogueElementBase element)
        {
            // Create the main dialogue box container
            Box boxContainer = UIHelper.CreateBox("DialogueBox");
            boxContainer.AddToClassList("DialogueBox");

            // Create the title container
            Box titleContainer = UIHelper.CreateBox("DialogueBoxTitle");

            Label label = UIHelper.CreateLabel(titleText, "LabelText");
            titleContainer.Add(label);

            // Add "Move Up", "Move Down", and "Remove" buttons
            titleContainer.Add(CreateMoveAndRemoveButtons(element));

            boxContainer.Add(titleContainer);

            return boxContainer;
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
                case DialogueNameData dialogueName:
                    AddDialogueName(dialogueName);
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

            _currentNodeData.AllDialogueElements.Remove(element);

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
