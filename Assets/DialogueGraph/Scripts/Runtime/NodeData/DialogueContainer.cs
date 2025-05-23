using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "DialogueContainer", menuName = "DialogueGraph/DialogueContainer")]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData>  NodeLinkDatas = new List<NodeLinkData>();

        // Node Data
        public StartData StartNodeData;
        public List<EndNodeData> EndNodesDatas = new List<EndNodeData>();
        public List<DialogueNodeData> DialogueNodesDatas = new List<DialogueNodeData>();
        public List<ChoiceNodeData> ChoiceNodesDatas = new List<ChoiceNodeData>();
        public List<BranchNodeData> BranchNodes = new List<BranchNodeData>();
        public List<EventNodeData> EventNodesDatas = new List<EventNodeData>();

        public List<BaseData> GetAllDatas
        {
            get
            {
                List<BaseData> tmpBaseDatas = new List<BaseData>();
                tmpBaseDatas.Add(StartNodeData);
                tmpBaseDatas.AddRange(EndNodesDatas);
                tmpBaseDatas.AddRange(EventNodesDatas);
                tmpBaseDatas.AddRange(BranchNodes);
                tmpBaseDatas.AddRange(DialogueNodesDatas);
                tmpBaseDatas.AddRange(ChoiceNodesDatas);

                return tmpBaseDatas;
            }
        }
    }
}
