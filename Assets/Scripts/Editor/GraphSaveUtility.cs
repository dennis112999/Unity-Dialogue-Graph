using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEditor;
using Dennis.Tools.DialogueGraph.Data;


namespace Dennis.Tools.DialogueGraph
{
    public class GraphSaveUtility
    {
        private DialogueView _dialogueView;
        private DialogueContainer _containerCache;

        private List<Edge> _edges => _dialogueView.edges.ToList();

        private List<BaseNode> _dialogueNodes => _dialogueView.nodes.Cast<BaseNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogueView dialogueView)
        {
            return new GraphSaveUtility
            {
                _dialogueView = dialogueView
            };
        }

        #region Save

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveGraph(string fileName)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(dialogueContainer)) return;

            // Auto Create folder
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            string assetPath = $"Assets/Resources/{fileName}.asset";

            DialogueContainer tempDialogueContainer = AssetDatabase.LoadAssetAtPath<DialogueContainer>(assetPath);

            if (dialogueContainer != null)
            {
                if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath) != null)
                {
                    EditorUtility.SetDirty(tempDialogueContainer);
                }
                else
                {
                    AssetDatabase.CreateAsset(dialogueContainer, assetPath);
                }
                AssetDatabase.SaveAssets();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Asset not found. Cannot modify the data.");
#endif
            }
        }

        /// <summary>
        /// Saves the nodes and edges
        /// </summary>
        /// <param name="dialogueContainer"></param>
        /// <returns></returns>
        private bool SaveNodes(DialogueContainer dialogueContainer)
        {
            if (!_edges.Any()) return false;

            // Save edges and node data
            try
            {
                SaveEdges(dialogueContainer);
                SaveNodesDatas(dialogueContainer);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SaveNodes failed: An error occurred while saving nodes. Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Saves the edges data into the DialogueContainer
        /// </summary>
        /// <param name="dialogueContainer">The target container to store edge data</param>
        private void SaveEdges(DialogueContainer dialogueContainer)
        {
            dialogueContainer.nodeLinkDatas.Clear();

            Edge[] connectedEdges = _edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Length; i++)
            {
                BaseNode outputNode = connectedEdges[i].output.node as BaseNode;
                BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

                if (outputNode == null || inputNode == null)
                {
                    Debug.LogWarning("Skipped an edge due to a null node reference.");
                    continue;
                }

                dialogueContainer.nodeLinkDatas.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    BasePortName = connectedEdges[i].output.portName,
                    TargetNodeGuid = inputNode.GUID,
                    TargetPortName = connectedEdges[i].input.portName,
                });
            }
        }


        /// <summary>
        /// Saves node data into the DialogueContainer
        /// </summary>
        /// <param name="dialogueContainer">The container to store the node data</param>
        private void SaveNodesDatas(DialogueContainer dialogueContainer)
        {
            // Clear existing data to prepare for new data
            dialogueContainer.StartDatas.Clear();
            dialogueContainer.EndDatas.Clear();

            foreach (var node in _dialogueNodes)
            {
                switch (node)
                {
                    case StartNode startNode:
                        dialogueContainer.StartDatas.Add(new StartData
                        {
                            NodeGuid = startNode.GUID,
                            Position = startNode.GetPosition().position,
                        });
                        break;

                    case EndNode endNode:
                        dialogueContainer.EndDatas.Add(new EndData
                        {
                            NodeGuid = endNode.GUID,
                            Position = endNode.GetPosition().position,
                        });
                        break;

                    default:
                        Debug.LogWarning($"Unhandled node type: {node.GetType()} - Node GUID: {node.GUID}");
                        break;
                }
            }
        }


        #endregion Save


    }
}
