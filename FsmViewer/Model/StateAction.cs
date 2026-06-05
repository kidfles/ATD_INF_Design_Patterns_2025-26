namespace Fsm.Model;

public sealed class StateAction
{
    public StateAction(StateActionKind kind, string description)
    {
        Kind = kind;
        Description = description;
    }

    public StateActionKind Kind { get; }
    public string Description { get; }
}
