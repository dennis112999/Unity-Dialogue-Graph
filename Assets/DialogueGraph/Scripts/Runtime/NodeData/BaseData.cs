using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    public class BaseData
    {
        public string NodeGuid;
        public Vector2 Position;
    }

    [System.Serializable]
    public class StartData : BaseData
    {

    }

    [System.Serializable]
    public class EndData : BaseData
    {

    }

    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string BasePortName;
        public string TargetNodeGuid;
        public string TargetPortName;
    }
}
