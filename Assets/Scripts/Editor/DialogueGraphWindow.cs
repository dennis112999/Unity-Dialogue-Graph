using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

using System;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueGraphWindow : EditorWindow
    {
        private DialogueView _dialogueView;
        private string _fileName = "New Narrative";

        [MenuItem("Graph/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraphWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_dialogueView);
        }

        /// <summary>
        /// Construct Graph View -> Only one time
        /// </summary>
        private void ConstructGraphView()
        {
            _dialogueView = new DialogueView(this)
            {
                name = "Dialogue Graph"
            };

            _dialogueView.StretchToParentSize();
            rootVisualElement.Add(_dialogueView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            // Create FileNameText
            var fileNameTextField = new TextField("File Name :");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            // Add Save Data Button and Load Data Button
            Button SaveDataButton = CreateButton("Save Data", () => RequestDataOperation(true));
            toolbar.Add(SaveDataButton);

            rootVisualElement.Add(toolbar);
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };

            Action updateMiniMapPosition = () =>
            {
                float windowWidth = _dialogueView.contentContainer.layout.width;

                if (float.IsNaN(windowWidth))
                {
                    Debug.LogError("Window width is NaN. Layout may not be updated.");
                }
                else
                {
                    var cards = _dialogueView.contentViewContainer.WorldToLocal(new Vector2(windowWidth - 210, 50));
                    miniMap.SetPosition(new Rect(cards.x, cards.y, 200, 140));
                }
            };

            // Schedule the initial position setup
            _dialogueView.schedule.Execute(updateMiniMapPosition).ExecuteLater(0);

            // Listen to the GeometryChangedEvent to update the position when the window size changes
            _dialogueView.RegisterCallback<GeometryChangedEvent>(evt => updateMiniMapPosition());

            _dialogueView.Add(miniMap);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
            }
            else
            {
                var saveUtility = GraphSaveUtility.GetInstance(_dialogueView);

                if(save)
                {
                    saveUtility.SaveGraph(_fileName);
                }
                else
                {
                    saveUtility.LoadGraph(_fileName);
                }
            }
        }

        #region UI

        private Button CreateButton(string buttonText, Action onClickAction)
        {
            Button button = new Button(onClickAction) { text = buttonText };
            return button;
        }

        #endregion UI
    }

}