using Fsm.Model;

namespace Fsm.Parsing;

public interface IFsmBuilder
{
    void AddStateDefinition(string id, string parentId, string name, string typeText);
    void AddTriggerDefinition(string id, string description);
    void AddActionDefinition(string ownerId, string description, string typeText);
    void AddTransitionDefinition(string id, string sourceId, string destinationId, string? triggerId, string guard);
    StateMachine Build();
}
