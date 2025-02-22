using System.Collections;
using System.Collections.Generic;
using System;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    public class BranchNodeData : BaseData
    {
        public List<ConditionData> ConditionDatas = new List<ConditionData>();
    }
}
