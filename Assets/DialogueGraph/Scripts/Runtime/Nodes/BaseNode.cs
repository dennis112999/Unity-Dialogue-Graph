#if UNITY_EDITOR

using Dennis.Tools.DialogueGraph.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dennis.Tools.DialogueGraph
{
    public class BaseNode : Node
    {
        protected string guid;
        public string GUID { get => guid; set => guid = value; }

        protected DialogueView graphView;
        protected DialogueGraphWindow editorWindow;
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

        public BaseNode()
        {
            // Load the style Sheets
            StyleSheet styleSheet = Resources.Load<StyleSheet>("Node");
            styleSheets.Add(styleSheet);
        }

        protected void RefeshUI()
        {
            RefreshExpandedState();
            RefreshPorts();
        }

        #region Port

        /// <summary>
        /// Adds an output port
        /// </summary>
        /// <param name="name">The display name</param>
        /// <param name="capacity">The connection capacity of the port (single or multi)</param>
        /// <returns>Created output port</returns>
        public Port AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capacity);
            outputPort.portName = name;
            outputContainer.Add(outputPort);
            return outputPort;
        }

        /// <summary>
        /// Adds an input port
        /// </summary>
        /// <param name="name">The display name</param>
        /// <param name="capacity">The connection capacity of the port (single or multi)</param>
        /// <returns>Created input port</returns>
        public Port AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port inputPort = GetPortInstance(Direction.Input, capacity);
            inputPort.portName = name;
            inputContainer.Add(inputPort);
            return inputPort;
        }

        /// <summary>
        /// Instantiates a port
        /// </summary>
        /// <param name="nodeDirection">The direction of the port (input or output)</param>
        /// <param name="capacity">The connection capacity of the port (single or multi)</param>
        /// <returns>Created port</returns>
        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        #endregion

        protected void AddComparisonValueField(ConditionData conditionData, Box boxContainer, ConditionType conditionType)
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                case ConditionType.GreaterOrEqual:
                case ConditionType.LessOrEqual:
                case ConditionType.Greater:
                case ConditionType.Less:
                    FloatField valueField = UIHelper.CreateFloatField(conditionData.ComparisonValue, newValue =>
                    {
                        conditionData.ComparisonValue = newValue;
                    });

                    boxContainer.Add(valueField);
                    break;

                default:
                case ConditionType.IsTrue:
                case ConditionType.IsFalse:
                    // No additional input field needed
                    break;
            }
        }
    }
}

#endif