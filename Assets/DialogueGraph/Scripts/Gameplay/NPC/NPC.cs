using UnityEngine;

using Dennis.Tools.DialogueGraph.Event;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private string _dialogueId = "DialogueId";

#if UNITY_EDITOR

        void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = 20;

            if (GUI.Button(new Rect(10, 10, 200, 50), "Trigger Dialogue", style))
            {
                Events.OnDialogueTriggered.Publish(_dialogueId);
            }
        }

#endif
    }
}
