using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;

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
            // To Do : Button Save / Load function
            toolbar.Add(new Button() { text = "Save Data" });
            toolbar.Add(new Button() { text = "Load Data" });

            rootVisualElement.Add(toolbar);
        }
    }

}