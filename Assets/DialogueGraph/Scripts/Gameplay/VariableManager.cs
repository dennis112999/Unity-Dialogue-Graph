using UnityEngine;
using Dennis.Tools.DialogueGraph.Event;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public static class VariableManager
    {
        private static VariableData _variableDataSO;

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize(VariableData variableData)
        {
            _variableDataSO = variableData;
            Events.OnVariableOperationEvents.Add(ApplyOperation);
        }

        /// <summary>
        /// Cleans up event listeners when VariableManager is no longer needed
        /// </summary>
        public static void Cleanup()
        {
            Events.OnVariableOperationEvents.Remove(ApplyOperation);
        }

        /// <summary>
        /// Applies an operation to a variable
        /// </summary>
        public static void ApplyOperation(VariableOperationData data)
        {
            if (_variableDataSO == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[VariableManager] VariableDataSO is not initialized. Call Initialize() first.");
#endif
                return;
            }

            if (!_variableDataSO.TryGetValue(data.VariableName, out float currentValue))
            {
#if UNITY_EDITOR
                Debug.LogError($"[VariableManager] Variable '{data.VariableName}' does not exist. Please check the VariableData SO.");
#endif
                return;
            }

            float newValue = VariableOperationProcessor.ProcessOperation(currentValue, data);
            _variableDataSO.SetValue(data.VariableName, newValue);
        }

        /// <summary>
        /// Evaluates whether a condition is met based on the given condition data.
        /// </summary>
        public static bool EvaluateCondition(ConditionData data)
        {
            if (_variableDataSO == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[VariableManager] VariableDataSO is not initialized. Call Initialize() first.");
#endif
                return false;
            }

            if (!_variableDataSO.TryGetValue(data.ConditionText, out float currentValue))
            {
#if UNITY_EDITOR
                Debug.LogError($"[VariableManager] Variable '{data.ConditionText}' does not exist. Please check the VariableData SO.");
#endif
                return false;
            }

            return ConditionProcessor.Evaluate(currentValue, data);
        }

        /// <summary>
        /// Retrieves all variable data.
        /// </summary>
        public static VariableData GetAllData()
        {
            if (_variableDataSO == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[VariableManager] VariableDataSO is not initialized. Call Initialize() first.");
#endif
                return null;
            }

            return _variableDataSO;
        }

        public static float TryGetValue(string id)
        {
            if (!_variableDataSO.TryGetValue(id, out float currentValue))
            {
#if UNITY_EDITOR
                Debug.LogError($"[VariableManager] Variable '{id}' does not exist. Please check the VariableData SO.");
#endif
                return 0f;
            }

            return currentValue;
        }

        public static bool TryGetBoolValue(string id)
        {
            return TryGetValue(id) == 1f;
        }
    }
}
