using UnityEngine;
using System.Linq;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueFlowHandler : MonoBehaviour
    {
        protected DialogueContainer DialogueContainer;

        /// <summary>
        /// Finds a node in the dialogue container by its GUID
        /// </summary>
        /// <param name="targetNodeGuid">The GUID of the target node</param>
        /// <returns>The node matching the specified GUID, or null if not found</returns>
        protected BaseData GetNodeByGuid(string targetNodeGuid)
        {
            return DialogueContainer.GetAllDatas.FirstOrDefault(node => node.NodeGuid == targetNodeGuid);
        }

        /// <summary>
        /// Finds a node in the dialogue container based on its input port
        /// </summary>
        /// <param name="nodePort">The node port to search for</param>
        /// <returns>The node matching the input port, or null if not found</returns>
        protected BaseData GetNodeByNodePort(DialogueDataPort nodePort)
        {
            return DialogueContainer.GetAllDatas.FirstOrDefault(node => node.NodeGuid == nodePort.InputGuid);
        }

        /// <summary>
        /// Gets the next node in the dialogue flow
        /// </summary>
        /// <param name="baseNodeData">The current base node</param>
        /// <returns>The next node in the dialogue flow, or null if not found</returns>
        protected BaseData GetNextNode(BaseData baseNodeData)
        {
            NodeLinkData nodeLinkData = DialogueContainer.NodeLinkDatas.FirstOrDefault(edge => edge.BaseNodeGuid == baseNodeData.NodeGuid);

            return nodeLinkData != null ? GetNodeByGuid(nodeLinkData.TargetNodeGuid) : null;
        }
    }
}
