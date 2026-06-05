namespace Fsm.Model;

public sealed class CompoundState : State
{
    private readonly List<State> _substates = new();

    public CompoundState(string id, string name) : base(id, name)
    {
    }

    public IReadOnlyList<State> Substates => _substates;

    internal void AddSubstate(State child)
    {
        child.Parent = this;
        _substates.Add(child);
    }

    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
