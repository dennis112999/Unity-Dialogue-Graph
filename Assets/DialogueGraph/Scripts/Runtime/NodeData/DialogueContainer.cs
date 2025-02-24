using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData>  NodeLinkDatas = new List<NodeLinkData>();

        // Node Data
        public List<StartData> StartDatas = new List<StartData>();
        public List<EndData> EndDatas = new List<EndData>();
        public List<DialogueNodeData> DialogueNodes = new List<DialogueNodeData>();
        public List<ChoiceNodeData> ChoiceNodes = new List<ChoiceNodeData>();
        public List<BranchNodeData> BranchNodes = new List<BranchNodeData>();
        public List<EventNodeData> EventNodes = new List<EventNodeData>();
    }
}
