using System.Collections.Generic;
using System;

using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    public class BranchNodeData : BaseData, IConditionHolder
    {
        public string TrueGuidNode;
        public string FalseGuidNode;

        [SerializeField] private List<ConditionData> conditionDatas = new List<ConditionData>();

        public List<ConditionData> ConditionDatas
        {
            get => conditionDatas;
            set => conditionDatas = value ?? new List<ConditionData>();
        }

    }
}
