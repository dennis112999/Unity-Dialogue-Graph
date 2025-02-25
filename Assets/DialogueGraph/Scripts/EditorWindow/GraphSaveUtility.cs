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
                    tempDialogueContainer.NodeLinkDatas.Clear();
                    tempDialogueContainer.StartNodeDatas.Clear();
                    tempDialogueContainer.EndNodesDatas.Clear();
                    tempDialogueContainer.DialogueNodesDatas.Clear();
                    tempDialogueContainer.ChoiceNodesDatas.Clear();
                    tempDialogueContainer.EventNodesDatas.Clear();

                    tempDialogueContainer.NodeLinkDatas = dialogueContainer.NodeLinkDatas;
                    tempDialogueContainer.StartNodeDatas = dialogueContainer.StartNodeDatas;
                    tempDialogueContainer.EndNodesDatas = dialogueContainer.EndNodesDatas;
                    tempDialogueContainer.DialogueNodesDatas = dialogueContainer.DialogueNodesDatas;
                    tempDialogueContainer.ChoiceNodesDatas = dialogueContainer.ChoiceNodesDatas;
                    tempDialogueContainer.EventNodesDatas = dialogueContainer.EventNodesDatas;

                    EditorUtility.SetDirty(tempDialogueContainer);
                }
                else
                {
                    AssetDatabase.CreateAsset(dialogueContainer, assetPath);
                }
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Save Successful", $"File '{fileName}.asset' has been saved successfully!", "OK");
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
            dialogueContainer.NodeLinkDatas.Clear();

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

                dialogueContainer.NodeLinkDatas.Add(new NodeLinkData
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
            dialogueContainer.StartNodeDatas.Clear();
            dialogueContainer.EndNodesDatas.Clear();
            dialogueContainer.DialogueNodesDatas.Clear();
            dialogueContainer.ChoiceNodesDatas.Clear();
            dialogueContainer.BranchNodes.Clear();
            dialogueContainer.EventNodesDatas.Clear();

            foreach (var node in _dialogueNodes)
            {
                switch (node)
                {
                    case StartNode startNode:
                        dialogueContainer.StartNodeDatas.Add(new StartData
                        {
                            NodeGuid = startNode.GUID,
                            Position = startNode.GetPosition().position,
                        });
                        break;

                    case EndNode endNode:
                        dialogueContainer.EndNodesDatas.Add(new EndData
                        {
                            NodeGuid = endNode.GUID,
                            Position = endNode.GetPosition().position,
                        });
                        break;

                    case DialogueNode dialogueNode:
                        dialogueContainer.DialogueNodesDatas.Add(SaveDialogueNodeData(dialogueNode));
                        break;

                    case ChoiceNode choiceNode:
                        dialogueContainer.ChoiceNodesDatas.Add(new ChoiceNodeData
                        {
                            NodeGuid = choiceNode.GUID,
                            Position = choiceNode.GetPosition().position,
                            Text = choiceNode.CurrentNodeData.Text,
                            AudioClip = choiceNode.CurrentNodeData.AudioClip,
                            ChoiceNodeFailAction = choiceNode.CurrentNodeData.ChoiceNodeFailAction,
                            ConditionDatas = choiceNode.CurrentNodeData.ConditionDatas,
                        });
                        break;

                    case BranchNode branchNode:
                        dialogueContainer.BranchNodes.Add(new BranchNodeData
                        {
                            NodeGuid = branchNode.GUID,
                            Position = branchNode.GetPosition().position,
                            ConditionDatas = branchNode.CurrentNodeData.ConditionDatas,
                        });
                        break;

                    case EventNode eventNode:
                        dialogueContainer.EventNodesDatas.Add(new EventNodeData
                        {
                            NodeGuid = eventNode.GUID,
                            Position = eventNode.GetPosition().position,
                            VariableOperationDatas = eventNode.CurrentNodeData.VariableOperationDatas,
                        });
                        break;

                    default:
                        Debug.LogWarning($"Unhandled node type: {node.GetType()} - Node GUID: {node.GUID}");
                        break;
                }
            }
        }

        /// <summary>
        /// Saves the DialogueNode data into a DialogueNodeData object
        /// </summary>
        /// <param name="node">The DialogueNode to save</param>
        /// <returns>The saved DialogueNodeData object</returns>
        private DialogueNodeData SaveDialogueNodeData(DialogueNode node)
        {
            // Initialize new DialogueNodeData
            DialogueNodeData dialogueData = new DialogueNodeData
            {
                NodeGuid = node.GUID,
                Position = node.GetPosition().position,
                AllDialogueElements = node.CurrentNodeData.AllDialogueElements,
            };

            // Process each port in the node
            foreach (DialogueDataPort port in node.CurrentNodeData.DialogueDataPorts)
            {
                DialogueDataPort portData = new DialogueDataPort
                {
                    PortGuid = port.PortGuid,
                    OutputGuid = string.Empty,
                    InputGuid = string.Empty
                };

                // Match the edge to set Output and Input GUIDs
                foreach (Edge edge in _edges)
                {
                    if (edge.output.portName == port.PortGuid)
                    {
                        portData.OutputGuid = (edge.output.node as BaseNode)?.GUID ?? string.Empty;
                        portData.InputGuid = (edge.input.node as BaseNode)?.GUID ?? string.Empty;
                    }
                }

                dialogueData.DialogueDataPorts.Add(portData);
            }

            return dialogueData;
        }

        #endregion Save

        #region Load

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<DialogueContainer>(fileName);

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File not Found!", "Target dialogue graph file does not exists!", "OK");
                return;
            }

            ClearGraph();
            GenerateNodes(_containerCache);
            ConnectNodes(_containerCache);
        }

        /// <summary>
        /// Clear View Graph
        /// </summary>
        public void ClearGraph()
        {
            _edges.ForEach(edge => _dialogueView.RemoveElement(edge));

            foreach (BaseNode node in _dialogueNodes)
            {
                _dialogueView.RemoveElement(node);
            }
        }

        /// <summary>
        /// Generate Nodes
        /// </summary>
        /// <param name="dialogueContainer"></param>
        private void GenerateNodes(DialogueContainer dialogueContainer)
        {
            // Start
            foreach (StartData node in dialogueContainer.StartNodeDatas)
            {
                _dialogueView.CreateStartNode(node.Position, node.NodeGuid);
            }

            // End Node 
            foreach (EndData node in dialogueContainer.EndNodesDatas)
            {
                _dialogueView.CreateEndNode(node.Position, node.NodeGuid);
            }

            // Dialogue Node
            foreach (DialogueNodeData node in dialogueContainer.DialogueNodesDatas)
            {
                _dialogueView.CreateDialogueNode(node.Position, node);
            }

            // Choice Node
            foreach (ChoiceNodeData node in dialogueContainer.ChoiceNodesDatas)
            {
                _dialogueView.CreateChoiceNode(node.Position, node);
            }

            // Branch Node
            foreach (BranchNodeData node in dialogueContainer.BranchNodes)
            {
                _dialogueView.CreateBranchNode(node.Position, node);
            }

            // Event Node
            foreach (EventNodeData node in dialogueContainer.EventNodesDatas)
            {
                _dialogueView.CreateEventNode(node.Position, node);
            }
        }

        /// <summary>
        /// Connects all nodes
        /// </summary>
        /// <param name="dialogueContainer"></param>
        private void ConnectNodes(DialogueContainer dialogueContainer)
        {
            // Make connection for all nodes.
            for (int i = 0; i < _dialogueNodes.Count; i++)
            {
                List<NodeLinkData> connections = dialogueContainer.NodeLinkDatas
                    .Where(edge => edge.BaseNodeGuid == _dialogueNodes[i].GUID)
                    .ToList();

                if (_dialogueNodes[i].outputContainer == null || !_dialogueNodes[i].outputContainer.Children().Any())
                {
                    if (_dialogueNodes[i] is EndNode) continue;

                    string nodeTitle = _dialogueNodes[i].title;
                    string nodeType = _dialogueNodes[i].GetType().Name;
                    int outputPortCount = _dialogueNodes[i].outputContainer?.Children().Count() ?? 0;

                    Debug.LogWarning($"[DEBUG] Node Issue Detected: \n" +
                                     $"- GUID: {_dialogueNodes[i].GUID} \n" +
                                     $"- Title: {nodeTitle} \n" +
                                     $"- Type: {nodeType} \n" +
                                     $"- Current Output Port Count: {outputPortCount}");

                    continue;
                }

                List<Port> allOutputPorts = _dialogueNodes[i].outputContainer
                    .Children()
                    .Where(x => x is Port)
                    .Cast<Port>()
                    .ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    BaseNode targetNode = _dialogueNodes.FirstOrDefault(node => node.GUID == targetNodeGuid);

                    if (targetNode == null)
                    {
                        Debug.LogWarning($"Target node with GUID {targetNodeGuid} not found.");
                        continue;
                    }

                    if (targetNode.inputContainer == null || !targetNode.inputContainer.Children().Any())
                    {
                        Debug.LogWarning($"Target node {targetNode.GUID} has no input ports.");
                        continue;
                    }

                    foreach (Port item in allOutputPorts)
                    {
                        if (item.portName == connections[j].BasePortName)
                        {
                            LinkNodes(item, (Port)targetNode.inputContainer[0]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a link between two nodes
        /// </summary>
        /// <param name="outputPort"></param>
        /// <param name="inputPort"></param>
        private void LinkNodes(Port outputPort, Port inputPort)
        {
            // Create a new edge to connect the two ports
            Edge tempEdge = new Edge
            {
                output = outputPort,
                input = inputPort
            };

            // Connect the ports to the edge
            try
            {
                tempEdge.input.Connect(tempEdge);
                tempEdge.output.Connect(tempEdge);

                _dialogueView.Add(tempEdge);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LinkNodes failed: An error occurred while linking nodes. Exception: {ex.Message}");
            }
        }

        #endregion Load

    }
}
