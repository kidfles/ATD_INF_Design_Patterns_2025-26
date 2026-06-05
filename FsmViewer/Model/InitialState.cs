namespace Fsm.Model;

public sealed class InitialState : State
{
    public InitialState(string id, string name) : base(id, name)
    {
    }

    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
