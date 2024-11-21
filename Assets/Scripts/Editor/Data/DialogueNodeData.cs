using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    public class DialogueNodeData : BaseData
    {
        public List<DialogueBoxData> DialogueBoxes = new List<DialogueBoxData>();
    }

    [System.Serializable]
    public class DialogueBoxData
    {
        public string Text;

        public DialogueBoxData(string text = "default")
        {
            Text = text;
        }
    }
}
