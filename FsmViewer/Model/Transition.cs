namespace Fsm.Model;

public sealed class Transition
{
    public Transition(
        string id,
        State source,
        State destination,
        Trigger? trigger,
        string guard,
        Effect? effect)
    {
        Id = id;
        Source = source;
        Destination = destination;
        Trigger = trigger;
        Guard = guard;
        Effect = effect;
    }

    public string Id { get; }
    public State Source { get; }
    public State Destination { get; }
    public Trigger? Trigger { get; }
    public string Guard { get; }
    public Effect? Effect { get; }

    public bool IsAutomatic => Trigger is null;
    public bool HasGuard => Guard.Length > 0;
    public bool IsSelfTransition => ReferenceEquals(Source, Destination);

    public void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
