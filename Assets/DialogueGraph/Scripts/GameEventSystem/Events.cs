using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph.Event
{
    public static partial class Events
    {
        public static readonly GameEvent<string> OnDialogueTriggered = new GameEvent<string>();
        public static readonly GameEvent<VariableOperationData> OnVariableOperationEvents = new GameEvent<VariableOperationData>();
    }
}