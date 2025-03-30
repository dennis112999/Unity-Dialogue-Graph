using Dennis.Tools.DialogueGraph.Event;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class StageEventInteractable : BaseInteractable
    {
        public override void Interact()
        {
            if (HasInteracted) return;

            Events.OnDialogueTriggered.Publish(interactionId);
        }
    }
}
