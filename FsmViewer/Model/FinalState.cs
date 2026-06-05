namespace Fsm.Model;

public sealed class FinalState : State
{
    public FinalState(string id, string name) : base(id, name)
    {
    }

    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
