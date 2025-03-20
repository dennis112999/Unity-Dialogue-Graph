using System.Collections.Generic;
using System;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    public class EventNodeData : BaseData
    {
        public List<VariableOperationData> VariableOperationDatas = new List<VariableOperationData>();

        public List<DialogueEventSO> DialogueEventSOs = new List<DialogueEventSO>();
    }

    [Serializable]
    public class VariableOperationData
    {
        public string VariableName = "Variable Name";
        public VariableOperationType OperationType = VariableOperationType.SetTrue;
        public float Value = 0;
    }
}
