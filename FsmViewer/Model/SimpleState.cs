namespace Fsm.Model;

public sealed class SimpleState : State
{
    public SimpleState(string id, string name) : base(id, name)
    {
    }

    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
