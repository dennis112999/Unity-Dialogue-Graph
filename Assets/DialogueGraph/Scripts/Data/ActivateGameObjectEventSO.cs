using System.Collections.Generic;
using UnityEngine;

using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph.Sample
{
    [CreateAssetMenu(fileName = "ActivateGameObjectsEventSO", menuName = "Game/Events/ActivateGameObjectsByID")]
    public class ActivateGameObjectEventSO : DialogueEventSO
    {
        [SerializeField] private List<string> _targetIDs = new List<string>();

        public override void ExecuteEvent()
        {
            Debug.Log($"[DialogueEvent] Request Activate");
            Debug.Log($"[DialogueEvent] Request Activate: {_targetIDs.Count}");

            foreach (var id in _targetIDs)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ItemManager.Instance.ActivateItemById(id);

#if UNITY_EDITOR
                    Debug.Log($"[DialogueEvent] Request Activate: {id}");
#endif
                }
            }
        }
    }
}
