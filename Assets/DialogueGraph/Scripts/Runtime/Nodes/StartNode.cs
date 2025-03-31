#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;
using UnityEngine;

using System;

namespace Dennis.Tools.DialogueGraph
{
    public class StartNode : BaseNode
    {
        public StartNode() { }

        public StartNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            title = "Start";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = Guid.NewGuid().ToString();

            AddOutputPort("Next", Port.Capacity.Single);

            base.RefeshUI();
        }
    }
}

#endif
