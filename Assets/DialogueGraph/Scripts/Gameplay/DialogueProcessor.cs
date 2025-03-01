using Dennis.Tools.DialogueGraph.Data;
using System.Collections.Generic;
using UnityEngine;
using Dennis.Tools.DialogueGraph.UI;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueProcessor : DialogueFlowHandler
    {
        [SerializeField] private DialogueControllerUI _dialogueControllerUI;

        private List<DialogueElementBase> _baseContainers;
        private int _currentIndex = 0;

        public void StartDialogue()
        {
            _dialogueControllerUI.ShowDialogueUI(true);
            ProcessNodeType(GetNextNode(DialogueContainer.StartNodeData));
        }

        private void RunNode(StartData nodeData)
        {
            ProcessNodeType(GetNextNode(DialogueContainer.StartNodeData));
        }

        private void ProcessNodeType(BaseData baseNodeData)
        {
            switch (baseNodeData)
            {
                case StartData startNode:
                    RunNode(startNode);
                    break;

                case EndNodeData endNode:
                    break;

                case DialogueNodeData dialogueNode:
                    RunNode(dialogueNode);
                    break;

                case EventNodeData eventNode:
                    break;

                case BranchNodeData branchNode:
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogWarning($"Unhandled node type: {baseNodeData.GetType().Name}");
#endif
                    break;
            }
        }

        #region Dialogue Node

        private void RunNode(DialogueNodeData dialogueNode)
        {
            _baseContainers = dialogueNode.AllDialogueElements;
            _currentIndex = 0;

            _baseContainers.Sort(delegate (DialogueElementBase x, DialogueElementBase y)
            {
                return x.OrderIndex.CompareTo(y.OrderIndex);
            });

            ProcessDialogue();
        }

        private void ProcessDialogue()
        {
            for (int i = _currentIndex; i < _baseContainers.Count; i++)
            {
                _currentIndex = i + 1;
                if (_baseContainers[i] is DialogueImagesData)
                {
                    DialogueImagesData tmp = _baseContainers[i] as DialogueImagesData;
                    _dialogueControllerUI.SetImage(tmp.Sprite_Left, tmp.Sprite_Right);
                }
                else if (_baseContainers[i] is DialogueBoxData)
                {
                    DialogueBoxData tmp = _baseContainers[i] as DialogueBoxData;
                    _dialogueControllerUI.SetContentText(tmp.Text);

                    // TODO - Audio
                    break;
                }
            }
        }

        #endregion Dialogue Node
    }
}
