using System.Collections.Generic;

namespace Dennis.Tools.DialogueGraph.Data
{
    public interface IConditionHolder
    {
        List<ConditionData> ConditionDatas { get; set; }
    }
}
