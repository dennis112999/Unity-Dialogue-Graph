#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

using UnityEngine;

using Dennis.Tools.DialogueGraph.Data;

using System;

namespace Dennis.Tools.DialogueGraph
{
    public class EndNode : BaseNode
    {
        private EndNodeData _currentNodeData;

        public EndNodeData CurrentNodeData => _currentNodeData;

        public EndNode() { }

        public EndNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            _currentNodeData = new EndNodeData();

            title = "End Node";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            base.RefeshUI();
        }

        private void SetEndNodeTypeEnum()
        {
            EnumField endNodeTypeEnumField = UIHelper.CreateEnumField(_currentNodeData.EndNodeType, updatedValue => 
            { 
                _currentNodeData.EndNodeType = updatedValue; 
            });
            mainContainer.Add(endNodeTypeEnumField);
        }

        #region Init

        public void Init(EndNodeData endNodeData = null)
        {
            if(endNodeData != null) _currentNodeData = endNodeData;

            SetEndNodeTypeEnum();
        }

        #endregion Init
    }

}

#endif