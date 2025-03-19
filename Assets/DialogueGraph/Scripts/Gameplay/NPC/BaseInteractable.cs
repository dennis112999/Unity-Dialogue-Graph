using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string interactionId;
        [SerializeField] protected bool autoTriggerOnEnter = false;

        public string InteractionId => interactionId;
        public bool AutoTriggerOnEnter => autoTriggerOnEnter;
        public bool HasInteracted => VariableManager.TryGetBoolValue(interactionId);

        public abstract void Interact();
    }
}
