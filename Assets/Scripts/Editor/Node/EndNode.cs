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

            // Set node capabilities to Unmovable and Undeletable
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;

            AddInputPort("Previous", Port.Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();
        }
    }

}