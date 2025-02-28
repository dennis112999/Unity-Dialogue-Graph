using UnityEditor.Experimental.GraphView;
using UnityEngine;

using System;

namespace Dennis.Tools.DialogueGraph
{
    public class EndNode : BaseNode
    {
        public EndNode() { }

        public EndNode(Vector2 position, DialogueGraphWindow editorWindow, DialogueView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            title = "End";
            SetPosition(new Rect(position, defaultNodeSize));
            guid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            base.RefeshUI();
        }
        }
    }

}