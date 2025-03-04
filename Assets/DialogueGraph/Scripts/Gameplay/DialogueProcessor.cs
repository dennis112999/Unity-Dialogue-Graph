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

        private void ExecuteStartNode()
        {
            ProcessNodeType(GetNextNode(DialogueContainer.StartNodeData));
        }

        private void ProcessNodeType(BaseData baseNodeData)
        {
            switch (baseNodeData)
            {
                case StartData startNodeData:
                    ExecuteStartNode();
                    break;

                case EndNodeData endNodeData:
                    ExecuteEndNode(endNodeData);
                    break;

                    break;

                    break;

                case BranchNodeData branchNode:
                case BranchNodeData branchNodeData:
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

        #region End Node

        private void ExecuteEndNode(EndNodeData endNodeData)
        {
            switch (endNodeData.EndNodeType)
            {
                case EndNodeType.End:
                    // Close the DialogueUI 
                    _dialogueControllerUI.ShowDialogueUI(false);
                    break;

                case EndNodeType.RetrunToStart:
                    // Return to the Start Node
                    ExecuteStartNode();
                    break;

                case EndNodeType.Repeat:
                    // Repeat the current dialogue node 
                    ProcessNodeType(GetNodeByGuid(_currentDialogueNodeData.NodeGuid));
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogWarning($"Unhandled EndNodeType: {endNodeData.EndNodeType}");
#endif
                    break;
            }
        }

        #endregion End Node
    }
}
