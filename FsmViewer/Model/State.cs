namespace Fsm.Model;

public abstract class State
{
    private readonly List<StateAction> _actions = new();

    protected State(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }
    public string Name { get; }
    public CompoundState? Parent { get; internal set; }
    public IReadOnlyList<StateAction> Actions => _actions;

    internal void AddAction(StateAction action) => _actions.Add(action);

    public abstract void Accept(IFsmVisitor visitor);
}
