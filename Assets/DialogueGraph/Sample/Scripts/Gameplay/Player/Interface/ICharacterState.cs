using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{
    public interface ICharacterState
    {
        void EnterState(UnityChan2DController character);
        void UpdateState(UnityChan2DController character);
        void FixedUpdateState(UnityChan2DController character);
        void ExitState(UnityChan2DController character);
    }

    public class NormalState : ICharacterState
    {
        public void EnterState(UnityChan2DController character) { }

        public void UpdateState(UnityChan2DController character)
        {
            float x = Input.GetAxis("Horizontal");
            bool jump = Input.GetButtonDown("Jump");
            character.Move(x, jump);
        }

        public void FixedUpdateState(UnityChan2DController character)
        {
            character.CheckGroundStatus();
        }

        public void ExitState(UnityChan2DController character) { }
    }

    public class DamagedState : ICharacterState
    {
        public void EnterState(UnityChan2DController character)
        {
            character.StartCoroutine(character.HandleDamageRoutine());
        }

        public void UpdateState(UnityChan2DController character) { }

        public void FixedUpdateState(UnityChan2DController character) { }

        public void ExitState(UnityChan2DController character) { }
    }

    public class InvincibleState : ICharacterState
    {
        public void EnterState(UnityChan2DController character)
        {
            character.ActivateInvincibility();
        }

        public void UpdateState(UnityChan2DController character) 
        {
            float x = Input.GetAxis("Horizontal");
            bool jump = Input.GetButtonDown("Jump");
            character.Move(x, jump);
        }

        public void FixedUpdateState(UnityChan2DController character) { }

        public void ExitState(UnityChan2DController character) { }
    }

    public class DialogueState : ICharacterState
    {
        public void EnterState(UnityChan2DController character)
        {
            character.GetRigidbody2D().velocity = Vector2.zero;
            character.Move(0, false);
        }

        public void UpdateState(UnityChan2DController character)
        {

        }

        public void FixedUpdateState(UnityChan2DController character)
        {
            character.CheckGroundStatus();
        }

        public void ExitState(UnityChan2DController character)
        {

        }
    }

}