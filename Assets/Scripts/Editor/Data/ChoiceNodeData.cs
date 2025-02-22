using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    public class ChoiceNodeData : BaseData
    {
        public string Text;
        public AudioClip AudioClip;

        public ChoiceNodeFailAction ChoiceNodeFailAction = ChoiceNodeFailAction.Hide;

        public List<ConditionData> ConditionDatas = new List<ConditionData>();
    }

    [System.Serializable]
    public class ConditionData
    {
        public string ConditionText = "String Event";
        public ConditionType ConditionType = ConditionType.IsFalse;
        public float ComparisonValue = 0;
    }
}
