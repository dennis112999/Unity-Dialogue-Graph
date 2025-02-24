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
        public List<StartData> StartNodeDatas = new List<StartData>();
        public List<EndData> EndNodesDatas = new List<EndData>();
        public List<DialogueNodeData> DialogueNodesDatas = new List<DialogueNodeData>();
        public List<ChoiceNodeData> ChoiceNodesDatas = new List<ChoiceNodeData>();
        public List<BranchNodeData> BranchNodes = new List<BranchNodeData>();
        public List<EventNodeData> EventNodesDatas = new List<EventNodeData>();

        public List<BaseData> GetAllDatas
        {
            get
            {
                List<BaseData> tmpBaseDatas = new List<BaseData>();
                tmpBaseDatas.AddRange(EndNodesDatas);
                tmpBaseDatas.AddRange(StartNodeDatas);
                tmpBaseDatas.AddRange(EventNodesDatas);
                tmpBaseDatas.AddRange(BranchNodes);
                tmpBaseDatas.AddRange(DialogueNodesDatas);
                tmpBaseDatas.AddRange(ChoiceNodesDatas);

                return tmpBaseDatas;
            }
        }
    }
}
