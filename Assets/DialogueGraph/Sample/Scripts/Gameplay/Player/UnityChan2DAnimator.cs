using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public class UnityChan2DAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        public void SetHorizontal(float value)
        {
            _animator.SetFloat("Horizontal", value);
        }

        public void SetVertical(float value)
        {
            _animator.SetFloat("Vertical", value);
        }

        public void SetIsGround(bool isGround)
        {
            _animator.SetBool("isGround", isGround);
        }

        public void PlayDamageAnimation(bool isGround)
        {
            _animator.Play(isGround ? "Damage" : "AirDamage");
            _animator.Play("Idle");
        }

        public void SetJumpTrigger()
        {
            _animator.SetTrigger("Jump");
        }

        public void SetInvincibleMode()
        {
            _animator.SetTrigger("Invincible Mode");
        }
    }
}
