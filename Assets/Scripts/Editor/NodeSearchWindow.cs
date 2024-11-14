using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Dennis.Tools.DialogueGraph
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueView _dialogueView;
        private EditorWindow _editorWindow;
        private Texture2D _indentationIcon;

        public void Init(EditorWindow editorWindow, DialogueView dialogueView)
        {
            _dialogueView = dialogueView;
            _editorWindow = editorWindow;

            // Icon
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        #region ISearchWindowProvider

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))
                {
                    userData = new DialogueNode(), level = 2
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // Get the mouse pos
            var worldMousePos = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent,
                context.screenMousePosition - _editorWindow.position.position);
            var localMousePos = _dialogueView.contentContainer.WorldToLocal(worldMousePos);

            switch (SearchTreeEntry.userData)
            {
                case DialogueNode dialogueNode:
                    _dialogueView.CreateNode("Dualogue Node", localMousePos);
                    return true;

                default:
                    return false;
            }
        }

        #endregion ISearchWindowProvider
    }
}
