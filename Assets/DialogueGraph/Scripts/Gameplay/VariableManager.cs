using UnityEngine;

using Dennis.Tools.DialogueGraph.Event;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public class VariableManager : MonoBehaviour
    {
        public VariableData VariableDataSO;

        private void Awake()
        {
            Events.OnVariableOperationEvents.Add(ApplyOperation);
        }

        private void OnDestroy()
        {
            Events.OnVariableOperationEvents.Remove(ApplyOperation);
        }

        public void ApplyOperation(VariableOperationData data)
        {
            if (!VariableDataSO.TryGetValue(data.VariableName, out float currentValue))
            {
#if UNITY_EDITOR
                Debug.LogError($"[VariableManager] Variable '{data.VariableName}' does not exist. Please check the VariableData SO.");
#endif
                return;
            }

            float newValue = VariableOperationProcessor.ProcessOperation(currentValue, data);
            VariableDataSO.SetValue(data.VariableName, newValue);
        }
    }
}
