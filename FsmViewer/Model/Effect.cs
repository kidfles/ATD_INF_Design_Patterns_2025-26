namespace Fsm.Model;

public sealed class Effect
{
    public Effect(string description)
    {
        Description = description;
    }

    public string Description { get; }
}
