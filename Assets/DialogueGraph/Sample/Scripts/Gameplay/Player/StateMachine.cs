namespace Dennis.Tools.DialogueGraph.Sample
{
    public class StateMachine
    {
        private ICharacterState _currentState;

        public void Initialize(ICharacterState startingState, UnityChan2DController character)
        {
            _currentState = startingState;
            _currentState.EnterState(character);
        }

        public void ChangeState(ICharacterState newState, UnityChan2DController character)
        {
            _currentState.ExitState(character);
            _currentState = newState;
            _currentState.EnterState(character);
        }

        public void UpdateState(UnityChan2DController character)
        {
            _currentState.UpdateState(character);
        }

        public void FixedUpdateState(UnityChan2DController character)
        {
            _currentState.FixedUpdateState(character);
        }

        public ICharacterState GetCurrentState() => _currentState;
    }
}