using UnityEngine;
using System.Collections.Generic;

namespace Dennis.Tools.DialogueGraph.Data
{
    [CreateAssetMenu(fileName = "VariableData", menuName = "Game/Variable Data")]
    public class VariableData : ScriptableObject
    {
        public List<VariableEntry> Variables = new List<VariableEntry>();

        private Dictionary<string, float> _variableDict = new Dictionary<string, float>();

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            _variableDict.Clear();
            foreach (var entry in Variables)
            {
                _variableDict[entry.Name] = entry.Value;
            }
        }

        public bool TryGetValue(string variableName, out float value)
        {
            return _variableDict.TryGetValue(variableName, out value);
        }

        public void SetValue(string variableName, float newValue)
        {
            if (_variableDict.ContainsKey(variableName))
            {
                _variableDict[variableName] = newValue;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"[VariableData] Variable '{variableName}' not found.");
#endif
            }
        }

        public Dictionary<string, float> GetAllVariables()
        {
            return new Dictionary<string, float>(_variableDict);
        }
    }

    [System.Serializable]
    public class VariableEntry
    {
        public string Name;
        public float Value;
    }
}