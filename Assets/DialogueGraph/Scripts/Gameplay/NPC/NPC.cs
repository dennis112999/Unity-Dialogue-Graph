using Dennis.Tools.DialogueGraph.Event;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public class NPCInteractable : BaseInteractable
    {
        public override void Interact()
        {
            if (HasInteracted) return;

            Events.OnDialogueTriggered.Publish(interactionId);
        }
    }
}
