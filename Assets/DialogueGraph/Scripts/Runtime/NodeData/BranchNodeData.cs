using System.Collections.Generic;
using System;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    public class BranchNodeData : BaseData, IConditionHolder
    {
        public List<ConditionData> ConditionDatas { get; set; } = new List<ConditionData>();
    }
}
