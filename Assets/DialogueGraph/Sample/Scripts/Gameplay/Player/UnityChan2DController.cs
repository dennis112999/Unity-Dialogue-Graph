using System.Collections;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    /// <summary>
    /// Sample 2D Controller
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class UnityChan2DController : MonoBehaviour
    {
        private const float CenterY = 1.5f;

        [Header("Character Settings")]
        public float MaxSpeed = 10f;
        public float JumpPower = 1000f;

        [Header("Damage Settings")]
        public Vector2 BackwardForce = new Vector2(-0.5f, 0.5f);

        [Header("Ground Detection")]
        public LayerMask WhatIsGround;

        [Header("References")]
        private UnityChan2DAnimator _animatorController;
        private BoxCollider2D _boxCollider2D;
        private Rigidbody2D _rigidbody2D;

        public Rigidbody2D GetRigidbody2D()
        {
            return _rigidbody2D;
        }

        [SerializeField] private PlayerInteraction _playerInteraction;

        private bool _isGround;
        private StateMachine _stateMachine;
        public StateMachine StateMachine { get { return _stateMachine; } }

        #region MonoBehaviour

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            _stateMachine.UpdateState(this);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdateState(this);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "DamageObject" && _stateMachine.GetCurrentState() is NormalState)
            {
                _stateMachine.ChangeState(new DamagedState(), this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IPickupable pickupable = other.GetComponent<IPickupable>();
            if (pickupable != null)
            {
                pickupable.OnPickup();
            }
        }

        #endregion MonoBehaviour

        private void Initialize()
        {
            _stateMachine = new StateMachine();
            _stateMachine.Initialize(new NormalState(), this);

            _playerInteraction.Init(this);

            // References Initialize
            _animatorController = GetComponent<UnityChan2DAnimator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        #region FSM Management

        public void SetState(ICharacterState newState)
        {
            _stateMachine.ChangeState(newState, this);
        }

        #endregion FSM Management

        #region Movement

        public void Move(float move, bool jump)
        {
            HandleCharacterFlip(move);
            MoveCharacter(move);
            UpdateAnimationState(move);
            HandleJump(jump);
        }

        private void HandleCharacterFlip(float move)
        {
            if (Mathf.Approximately(move, 0)) return;
            transform.rotation = Quaternion.Euler(0, Mathf.Sign(move) == 1 ? 0 : 180, 0);
        }

        private void MoveCharacter(float move)
        {
            _rigidbody2D.velocity = new Vector2(move * MaxSpeed, _rigidbody2D.velocity.y);
        }

        private void UpdateAnimationState(float move)
        {
            _animatorController.SetHorizontal(move);
            _animatorController.SetVertical(_rigidbody2D.velocity.y);
            _animatorController.SetIsGround(_isGround);
        }

        private void HandleJump(bool jump)
        {
            if (!jump || !_isGround) return;
            _animatorController.SetJumpTrigger();
            _rigidbody2D.AddForce(Vector2.up * JumpPower);
        }

        #endregion Movement

        #region Ground Check

        public void CheckGroundStatus()
        {
            Vector2 pos = transform.position;
            Vector2 groundCheck = new Vector2(pos.x, pos.y - (CenterY * transform.localScale.y));
            Vector2 groundArea = new Vector2(_boxCollider2D.size.x * 0.49f, 0.05f);
            _isGround = Physics2D.OverlapArea(groundCheck + groundArea, groundCheck - groundArea, WhatIsGround);
            _animatorController.SetIsGround(_isGround);
        }

        #endregion Ground Check

        #region Damage System

        public IEnumerator HandleDamageRoutine()
        {
            _animatorController.PlayDamageAnimation(_isGround);

            Vector2 knockback = new Vector2(transform.right.x * BackwardForce.x, transform.up.y * BackwardForce.y);
            knockback = Vector2.ClampMagnitude(knockback, 5f);
            _rigidbody2D.velocity = knockback;

            yield return new WaitForSeconds(0.2f);
            while (!_isGround) yield return new WaitForFixedUpdate();

            SetState(new InvincibleState());
        }

        public void ActivateInvincibility()
        {
            _animatorController.SetInvincibleMode();
        }

        public void OnFinishedInvincibleMode()
        {
            SetState(new NormalState());
        }

        #endregion Damage System
    }
}
