using UnityEngine;
using System.Collections.Generic;

namespace Dennis.Tools.DialogueGraph.Data
{
    [CreateAssetMenu(fileName = "DialogueContainerDatabase", menuName = "DialogueGraph/DialogueContainerDatabase")]
    public class DialogueContainerDatabase : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public string Id;
            public DialogueContainer Container;
        }

        public List<Entry> dialogueEntries = new List<Entry>();

        private Dictionary<string, DialogueContainer> containerMap;

        public DialogueContainer GetContainerById(string id)
        {
            if (containerMap == null)
            {
                containerMap = new Dictionary<string, DialogueContainer>();
                foreach (var entry in dialogueEntries)
                {
                    if (!containerMap.ContainsKey(entry.Id))
                    {
                        containerMap[entry.Id] = entry.Container;
                    }
                }
            }

            if (containerMap.TryGetValue(id, out var container))
            {
                return container;
            }

#if UNITY_EDITOR
            Debug.LogWarning($"DialogueContainer with ID '{id}' not found in DialogueContainerDatabase.");
#endif
            return null;
        }
    }
}
