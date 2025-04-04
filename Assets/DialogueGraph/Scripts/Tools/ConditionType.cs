namespace Dennis.Tools.DialogueGraph
{
    public enum ConditionType
    {
        IsTrue,          
        IsFalse,         
        Equal,           
        GreaterOrEqual,  
        LessOrEqual,     
        Greater,         
        Less             
    }

    public enum ChoiceNodeFailAction
    {
        Hide,
        Disable
    }

    public enum VariableOperationType
    {
        SetTrue,
        SetFalse,
        Add,
        Subtract,
        Multiply,
        Divide,
    }
}