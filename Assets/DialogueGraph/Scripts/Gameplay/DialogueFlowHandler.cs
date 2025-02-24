using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueFlowHandler : MonoBehaviour
    {
        [SerializeField] protected DialogueContainer DialogueContainer;

        protected BaseData GetNodeByGuid(string targetNodeGuid)
        {
            return DialogueContainer.GetAllDatas.Find(node => node.NodeGuid == targetNodeGuid);
        }

        protected BaseData GetNodeByNodePort(DialogueData_Port nodePort)
        {
            return DialogueContainer.GetAllDatas.Find(node => node.NodeGuid == nodePort.InputGuid);
        }

        protected BaseData GetNextNode(BaseData baseNodeData)
        {
            NodeLinkData nodeLinkData = DialogueContainer.NodeLinkDatas.Find(egde => egde.BaseNodeGuid == baseNodeData.NodeGuid);

            return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
        }
    }
}
