using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionRange = 1.5f;
        [SerializeField] private LayerMask npcLayerMask;
        [SerializeField] private GameObject interactionHintUI;

        private StateMachine _stateMachine;
        private UnityChan2DController _unityChan2DController;
        private Collider2D _lastDetectedObject;

        public void Init(UnityChan2DController unityChan2DController)
        {
            _unityChan2DController = unityChan2DController;
            _stateMachine = unityChan2DController.StateMachine;
        }

        private void Update()
        {
            if (!CanInteract()) return;

            DetectNearbyInteractable();

            if (Input.GetKeyDown(KeyCode.Q) && _lastDetectedObject != null)
            {
                AttemptInteract();
            }
        }

        private bool CanInteract()
        {
            return _stateMachine.GetCurrentState() is NormalState;
        }

        private void DetectNearbyInteractable()
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, npcLayerMask);
            _lastDetectedObject = hit;

            if (hit != null && hit.TryGetComponent(out BaseInteractable interactable))
            {
                // If auto-trigger is enabled and interaction hasn't happened yet
                if (interactable.AutoTriggerOnEnter && !interactable.HasInteracted)
                {
                    AutoTriggerInteract(interactable);
                    return;
                }

                // Show interaction hint UI if it's a manual interaction and not yet interacted
                interactionHintUI.SetActive(!interactable.HasInteracted);
            }
            else
            {
                // Hide the interaction hint if no interactable is nearby
                interactionHintUI.SetActive(false);
            }
        }

        private void AttemptInteract()
        {
            if (_lastDetectedObject == null) return;

            if (_lastDetectedObject.TryGetComponent(out IInteractable interactable))
            {
                PerformInteraction(interactable);
            }
        }

        private void AutoTriggerInteract(BaseInteractable interactable)
        {
            PerformInteraction(interactable);
        }

        /// <summary>
        /// Common interaction logic: stops player, sets dialogue state, and triggers interaction.
        /// </summary>
        private void PerformInteraction(IInteractable interactable)
        {
            _unityChan2DController.SetState(new DialogueState());
            interactable.Interact();
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
#endif
    }
}
