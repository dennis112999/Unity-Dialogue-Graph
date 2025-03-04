using UnityEngine;

using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public static class VariableOperationProcessor
    {
        /// <summary>
        /// Processes a variable operation based on the specified operation type
        /// </summary>
        /// <param name="currentValue">The current value of the variable before the operation is applied</param>
        /// <param name="data">The operation data containing the operation type and value</param>
        /// <returns>updated variable value after applying the operation</returns>
        public static float ProcessOperation(float currentValue, VariableOperationData data)
        {
            switch (data.OperationType)
            {
                /// <summary>
                /// Sets the variable to `1` (true state)
                /// </summary>
                case VariableOperationType.SetTrue:
                    return SetTrue();

                /// <summary>
                /// Sets the variable to `0` (false state)
                /// </summary>
                case VariableOperationType.SetFalse:
                    return SetFalse();

                /// <summary>
                /// Adds the specified value to the current variable value
                /// </summary>
                case VariableOperationType.Add:
                    return MathsUtility.Add(currentValue, data.Value);

                /// <summary>
                /// Subtracts the specified value from the current variable value
                /// </summary>
                case VariableOperationType.Subtract:
                    return MathsUtility.Subtract(currentValue, data.Value);

                /// <summary>
                /// Multiplies the current variable value by the specified value
                /// </summary>
                case VariableOperationType.Multiply:
                    return MathsUtility.Multiply(currentValue, data.Value);

                /// <summary>
                /// Divides the current variable value by the specified value
                /// </summary>
                case VariableOperationType.Divide:
                    return MathsUtility.Divide(currentValue, data.Value, data.VariableName);

                default:
#if UNITY_EDITOR
                    Debug.LogError($"[VariableOperationProcessor] Unsupported operation type: {data.OperationType}");
#endif
                    return currentValue;
            }
        }


        private static float SetTrue() => 1;

        private static float SetFalse() => 0;
    }
}
