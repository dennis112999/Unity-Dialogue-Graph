using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    [System.Serializable]
    public class ItemEntry
    {
        public string id;
        public GameObject obj;
    }


    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance { get; private set; }

        [SerializeField] private List<ItemEntry> items = new List<ItemEntry>();
        private Dictionary<string, GameObject> itemMap = new Dictionary<string, GameObject>();

        private void Awake()
        {
            Instance = this;
            itemMap.Clear();

            foreach (var entry in items)
            {
                if (!string.IsNullOrEmpty(entry.id) && entry.obj != null)
                {
                    itemMap[entry.id] = entry.obj;
                }
            }
        }

        public void ActivateItemById(string id)
        {
            if (itemMap.TryGetValue(id, out var obj))
            {
                obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"[ItemManager] ID '{id}' not found.");
            }
        }

        public GameObject GetItem(string id) => itemMap.TryGetValue(id, out var obj) ? obj : null;
    }
}
