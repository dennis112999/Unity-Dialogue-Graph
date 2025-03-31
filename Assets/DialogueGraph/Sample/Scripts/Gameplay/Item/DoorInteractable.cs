using Dennis.Tools.DialogueGraph.Event;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public class DoorInteractable : BaseInteractable
    {
        public override void Interact()
        {
            if (HasInteracted) return;

            Events.OnDialogueTriggered.Publish(interactionId);
        }
    }
}