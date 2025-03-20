using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEditor;
using Dennis.Tools.DialogueGraph.Data;
using System;
using Dennis.Tools.DialogueGraph.Utilities;

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

        public void SaveGraph(DialogueContainer dialogueContainer)
        {
            if (!SaveNodes(dialogueContainer)) return;
            if (!ValidateGraphBeforeSaving(dialogueContainer)) return;

            EditorUtility.SetDirty(dialogueContainer);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Save Successful", $"File '{dialogueContainer.name}.asset' has been saved successfully!", "OK");
        }

        private bool ValidateGraphBeforeSaving(DialogueContainer dialogueContainer)
        {
            // Check if StartNode exists before saving
            if (dialogueContainer.StartNodeData == null)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Save Warning", "The graph must contain a Start Node before saving.", "OK");
#endif
                return false;
            }

            // Check if at least one EndNode exists before saving
            if (dialogueContainer.EndNodesDatas.Count == 0)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Save Warning", "The graph must contain at least one End Node before saving.", "OK");
#endif
                return false;
            }

            // Ensure each DialogueNode has a `DialogueBoxData` as its last element
            foreach (var dialogueNodeData in dialogueContainer.DialogueNodesDatas)
            {
                if (!dialogueNodeData.AllDialogueElements.Any() ||
                    !(dialogueNodeData.AllDialogueElements.Last() is DialogueBoxData))
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Save Warning", "Each DialogueNode must end with a DialogueBoxData.", "OK");
#endif
                    return false;
                }
            }

            return true;
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
            catch (Exception ex)
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
            dialogueContainer.StartNodeData = null;
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
                        dialogueContainer.StartNodeData = new StartData
                        {
                            NodeGuid = startNode.GUID,
                            Position = startNode.GetPosition().position,
                        };
                        break;

                    case EndNode endNode:
                        dialogueContainer.EndNodesDatas.Add(new EndNodeData
                        {
                            NodeGuid = endNode.GUID,
                            Position = endNode.GetPosition().position,
                            EndNodeType = endNode.CurrentNodeData.EndNodeType
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
                        List<Edge> tmpEdges = _edges.Where(x => x.output.node == node).Cast<Edge>().ToList();

                        Edge trueOutput = tmpEdges.FirstOrDefault(x => x.output.portName == "True");
                        Edge falseOutput = tmpEdges.FirstOrDefault(x => x.output.portName == "False");

                        dialogueContainer.BranchNodes.Add(new BranchNodeData
                        {
                            NodeGuid = branchNode.GUID,
                            Position = branchNode.GetPosition().position,
                            TrueGuidNode = GetNodeGuidFromEdge(trueOutput),
                            FalseGuidNode = GetNodeGuidFromEdge(falseOutput),
                            ConditionDatas = branchNode.CurrentNodeData.ConditionDatas,
                        });
                        break;

                    case EventNode eventNode:
                        dialogueContainer.EventNodesDatas.Add(new EventNodeData
                        {
                            NodeGuid = eventNode.GUID,
                            Position = eventNode.GetPosition().position,
                            VariableOperationDatas = eventNode.CurrentNodeData.VariableOperationDatas,
                            DialogueEventSOs = eventNode.CurrentNodeData.DialogueEventSOs
                                    .Where(e => e != null && AssetDatabase.Contains(e))
                                    .ToList()
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

                foreach (Edge edge in _edges)
                {
                    if (edge.output.portName == port.PortGuid)
                    {
                        portData.OutputGuid = (edge.output.node as BaseNode).GUID;
                        portData.InputGuid = (edge.input.node as BaseNode).GUID;
                    }
                }

                dialogueData.DialogueDataPorts.Add(portData);
            }

            return dialogueData;
        }

        private string GetNodeGuidFromEdge(Edge edge)
        {
            return edge?.input?.node is BaseNode baseNode && !string.IsNullOrEmpty(baseNode.GUID)
                ? baseNode.GUID
                : string.Empty;
        }

        #endregion Save

        #region Load

        public void LoadGraphByDialogueContainer(DialogueContainer dialogueContainer)
        {
            _containerCache = dialogueContainer;

            ClearGraph();
            GenerateNodes(_containerCache);
            ConnectNodes(_containerCache);
        }

        /// <summary>
        /// Clear View Graph
        /// </summary>
        public void ClearGraph()
        {
            _dialogueView.ResetGraph();
            
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
            // Start Node
            if (dialogueContainer.StartNodeData != null)
                _dialogueView.CreateStartNode(dialogueContainer.StartNodeData.Position, dialogueContainer.StartNodeData.NodeGuid);

            // End Node 
            foreach (EndNodeData node in dialogueContainer.EndNodesDatas)
            {
                _dialogueView.CreateEndNode(node.Position, DialogueCloneUtility.CloneEndNode(node));
            }

            // Dialogue Node
            foreach (DialogueNodeData node in dialogueContainer.DialogueNodesDatas)
            {
                _dialogueView.CreateDialogueNode(node.Position, DialogueCloneUtility.CloneDialogueNode(node));
            }

            // Choice Node
            foreach (ChoiceNodeData node in dialogueContainer.ChoiceNodesDatas)
            {
                _dialogueView.CreateChoiceNode(node.Position, DialogueCloneUtility.CloneChoiceNode(node));
            }

            // Branch Node
            foreach (BranchNodeData node in dialogueContainer.BranchNodes)
            {
                _dialogueView.CreateBranchNode(node.Position, DialogueCloneUtility.CloneBranchNode(node));
            }

            // Event Node
            foreach (EventNodeData node in dialogueContainer.EventNodesDatas)
            {
                _dialogueView.CreateEventNode(node.Position, DialogueCloneUtility.CloneEventNode(node));
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
