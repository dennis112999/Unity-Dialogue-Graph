using System;
using System.Collections.Generic;
using System.Linq;
using Dennis.Tools.DialogueGraph.Data;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Utilities
{
    public static class DialogueCloneUtility
    {
        /// <summary>
        /// Creates a deep copy of a EndNodeData instance
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static EndNodeData CloneEndNode(EndNodeData original)
        {
            return new EndNodeData
            {
                NodeGuid = original.NodeGuid,
                Position = original.Position,
                EndNodeType = original.EndNodeType
            };
        }

        /// <summary>
        /// Creates a deep copy of a DialogueNodeData instance
        /// </summary>
        public static DialogueNodeData CloneDialogueNode(DialogueNodeData original)
        {
            return new DialogueNodeData
            {
                NodeGuid = original.NodeGuid,
                Position = original.Position,
                AllDialogueElements = original.AllDialogueElements.Select(CloneElement).ToList(),
                DialogueDataPorts = original.DialogueDataPorts.Select(ClonePort).ToList()
            };
        }

        /// <summary>
        /// Creates a deep copy of a ChoiceNodeData instance.
        /// </summary>
        public static ChoiceNodeData CloneChoiceNode(ChoiceNodeData original)
        {
            return new ChoiceNodeData
            {
                NodeGuid = original.NodeGuid,
                Position = original.Position,
                Text = original.Text,
                AudioClip = original.AudioClip,
                ChoiceNodeFailAction = original.ChoiceNodeFailAction,
                ConditionDatas = original.ConditionDatas != null
                    ? new List<ConditionData>(original.ConditionDatas.Select(CloneCondition))
                    : new List<ConditionData>()
            };
        }

        /// <summary>
        /// Creates a deep copy of a BranchNodeData instance
        /// </summary>
        public static BranchNodeData CloneBranchNode(BranchNodeData original)
        {
            return new BranchNodeData
            {
                NodeGuid = original.NodeGuid,
                Position = original.Position,
                ConditionDatas = original.ConditionDatas != null
                    ? new List<ConditionData>(original.ConditionDatas.Select(CloneCondition))
                    : new List<ConditionData>()
            };
        }

        /// <summary>
        /// Creates a deep copy of an EventNodeData instance
        /// </summary>
        public static EventNodeData CloneEventNode(EventNodeData original)
        {
            return new EventNodeData
            {
                NodeGuid = original.NodeGuid,
                Position = original.Position,
                VariableOperationDatas = original.VariableOperationDatas != null
                    ? new List<VariableOperationData>(original.VariableOperationDatas.Select(CloneVariableOperation))
                    : new List<VariableOperationData>(),
                DialogueEventSOs = new List<DialogueEventSO>(original.DialogueEventSOs)
            };
        }

        /// <summary>
        /// Creates a deep copy of a DialogueElementBase instance
        /// </summary>
        public static DialogueElementBase CloneElement(DialogueElementBase element)
        {
            switch (element)
            {
                case DialogueBoxData box:
                    return new DialogueBoxData { OrderIndex = box.OrderIndex, Text = box.Text, AudioClip = box.AudioClip };

                case DialogueNameData name:
                    return new DialogueNameData { OrderIndex = name.OrderIndex, Name = name.Name };

                case DialogueImagesData images:
                    return new DialogueImagesData { OrderIndex = images.OrderIndex, 
                                                    Sprite_Left = images.Sprite_Left, Sprite_Right = images.Sprite_Right, 
                                                    SpeakerTypeLeft = images.SpeakerTypeLeft, 
                                                    SpeakerTypeRight = images.SpeakerTypeRight };

                default:
                    throw new ArgumentException($"Unknown dialogue element type: {element.GetType().Name}");
            }
        }

        /// <summary>
        /// Creates a deep copy of a DialogueDataPort instance
        /// </summary>
        public static DialogueDataPort ClonePort(DialogueDataPort port)
        {
            return new DialogueDataPort
            {
                PortGuid = port.PortGuid,
                InputGuid = port.InputGuid,
                OutputGuid = port.OutputGuid
            };
        }

        /// <summary>
        /// Creates a deep copy of a ConditionData instance
        /// </summary>
        public static ConditionData CloneCondition(ConditionData condition)
        {
            if (condition == null)
            {
                Debug.LogError(" CloneCondition() received NULL `ConditionData`!");
                return null;
            }

            return new ConditionData
            {
                ConditionText = condition.ConditionText,
                ConditionType = condition.ConditionType,
                ComparisonValue = condition.ComparisonValue
            };
        }

        /// <summary>
        /// Creates a deep copy of a VariableOperationData instance
        /// </summary>
        public static VariableOperationData CloneVariableOperation(VariableOperationData operation)
        {
            if (operation == null)
            {
                Debug.LogError(" CloneVariableOperation() received NULL `VariableOperationData`!");
                return null;
            }

            return new VariableOperationData
            {
                VariableName = operation.VariableName,
                OperationType = operation.OperationType,
                Value = operation.Value
            };
        }
    }
}
