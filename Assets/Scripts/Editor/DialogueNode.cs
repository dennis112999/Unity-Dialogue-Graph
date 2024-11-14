using UnityEditor.Experimental.GraphView;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueNode : Node
    {
        public string GUID;

        public string DialogueText;

        public bool EntryPoint = false;
    }
}
