using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueGraphWindow : EditorWindow
    {
        private DialogueView _dialogueView;

        [MenuItem("Graph/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraphWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
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
    }

}