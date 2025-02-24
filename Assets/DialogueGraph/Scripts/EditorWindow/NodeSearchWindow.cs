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

        private SearchTreeEntry AddNodeSearch(string name, BaseNode baseNode)
        {
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(name, _indentationIcon))
            {
                level = 2,
                userData = baseNode
            };

            return tmp;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),

                AddNodeSearch("Start",new StartNode()),
                AddNodeSearch("End",new EndNode()),
                AddNodeSearch("Dialogue Node",new DialogueNode()),
                AddNodeSearch("Choice Node",new ChoiceNode()),
                AddNodeSearch("Branch Node",new BranchNode()),
                AddNodeSearch("Event Node",new EventNode()),
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
                case StartNode node:
                    _dialogueView.CreateStartNode(localMousePos);
                    return true;

                case EndNode node:
                    _dialogueView.CreateEndNode(localMousePos);
                    return true;

                case DialogueNode node:
                    _dialogueView.CreateDialogueNode(localMousePos);
                    return true;

                case ChoiceNode node:
                    _dialogueView.CreateChoiceNode(localMousePos);
                    return true;

                case BranchNode node:
                    _dialogueView.CreateBranchNode(localMousePos);
                    return true;

                case EventNode node:
                    _dialogueView.CreateEventNode(localMousePos);
                    return true;

                default:
                    return false;
            }
        }

        #endregion ISearchWindowProvider
    }
}
