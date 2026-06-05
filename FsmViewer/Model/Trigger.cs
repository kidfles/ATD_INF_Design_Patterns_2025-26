namespace Fsm.Model;

public sealed class Trigger
{
    public Trigger(string id, string description)
    {
        Id = id;
        Description = description;
    }

    public string Id { get; }
    public string Description { get; }
}
