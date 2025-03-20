using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DialogueEventSO", menuName = "Game/DialogueEventSO")]
    public class DialogueEventSO : ScriptableObject
    {
        public virtual void ExecuteEvent()
        {
#if UNITY_EDITOR
            Debug.Log("Event was executed");
#endif
        }
    }
}