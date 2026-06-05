using Fsm.Model;

namespace Fsm.Parsing;

public sealed class FsmObjectBuilder : IFsmBuilder
{
    private readonly List<StateDef> _states = new();
    private readonly List<TriggerDef> _triggers = new();
    private readonly List<ActionDef> _actions = new();
    private readonly List<TransitionDef> _transitions = new();

    public void AddStateDefinition(string id, string parentId, string name, string typeText) =>
        _states.Add(new StateDef(id, parentId, name, typeText));

    public void AddTriggerDefinition(string id, string description) =>
        _triggers.Add(new TriggerDef(id, description));

    public void AddActionDefinition(string ownerId, string description, string typeText) =>
        _actions.Add(new ActionDef(ownerId, description, typeText));

    public void AddTransitionDefinition(string id, string sourceId, string destinationId, string? triggerId, string guard) =>
        _transitions.Add(new TransitionDef(id, sourceId, destinationId, triggerId, guard));

    public StateMachine Build()
    {
        var states = CreateStates();
        var topLevel = ResolveParents(states);
        var (triggersById, triggersOrdered) = CreateTriggers();
        var effects = BindStateActionsAndCollectEffects(states);
        var transitions = CreateTransitions(states, triggersById, effects);
        return new StateMachine(topLevel, transitions, triggersOrdered);
    }

    private Dictionary<string, State> CreateStates()
    {
        var map = new Dictionary<string, State>();
        foreach (var def in _states)
        {
            if (map.ContainsKey(def.Id))
            {
                throw new FsmParseException($"Dubbele state-id '{def.Id}'.");
            }

            map[def.Id] = CreateState(def);
        }

        return map;
    }

    private static State CreateState(StateDef def) => def.TypeText.ToUpperInvariant() switch
    {
        "INITIAL" => new InitialState(def.Id, def.Name),
        "SIMPLE" => new SimpleState(def.Id, def.Name),
        "COMPOUND" => new CompoundState(def.Id, def.Name),
        "FINAL" => new FinalState(def.Id, def.Name),
        _ => throw new FsmParseException($"Onbekend state-type '{def.TypeText}'.")
    };

    private List<State> ResolveParents(Dictionary<string, State> states)
    {
        var topLevel = new List<State>();
        foreach (var def in _states)
        {
            var state = states[def.Id];
            if (def.ParentId == "_")
            {
                topLevel.Add(state);
                continue;
            }

            if (!states.TryGetValue(def.ParentId, out var parent))
            {
                throw new FsmParseException($"Onbekende parent '{def.ParentId}' voor state '{def.Id}'.");
            }

            if (parent is not CompoundState compound)
            {
                throw new FsmParseException($"Parent '{def.ParentId}' is geen compound state.");
            }

            compound.AddSubstate(state);
        }

        return topLevel;
    }

    private (Dictionary<string, Trigger> ById, List<Trigger> Ordered) CreateTriggers()
    {
        var byId = new Dictionary<string, Trigger>();
        var ordered = new List<Trigger>();
        foreach (var def in _triggers)
        {
            if (byId.ContainsKey(def.Id))
            {
                throw new FsmParseException($"Dubbele trigger-id '{def.Id}'.");
            }

            var trigger = new Trigger(def.Id, def.Description);
            byId[def.Id] = trigger;
            ordered.Add(trigger);
        }

        return (byId, ordered);
    }

    private Dictionary<string, Effect> BindStateActionsAndCollectEffects(Dictionary<string, State> states)
    {
        var effects = new Dictionary<string, Effect>();
        foreach (var def in _actions)
        {
            var type = def.TypeText.ToUpperInvariant();
            if (type == "TRANSITION_ACTION")
            {
                effects[def.OwnerId] = new Effect(def.Description);
                continue;
            }

            if (!states.TryGetValue(def.OwnerId, out var state))
            {
                throw new FsmParseException($"Onbekende state '{def.OwnerId}' voor actie.");
            }

            var kind = type switch
            {
                "ENTRY" => StateActionKind.Entry,
                "DO" => StateActionKind.Do,
                "EXIT" => StateActionKind.Exit,
                _ => throw new FsmParseException($"Onbekend actietype '{def.TypeText}'.")
            };

            state.AddAction(new StateAction(kind, def.Description));
        }

        return effects;
    }

    private List<Transition> CreateTransitions(
        Dictionary<string, State> states,
        Dictionary<string, Trigger> triggers,
        Dictionary<string, Effect> effects)
    {
        var result = new List<Transition>();
        foreach (var def in _transitions)
        {
            var source = ResolveState(states, def.SourceId);
            var destination = ResolveState(states, def.DestinationId);

            Trigger? trigger = null;
            if (def.TriggerId is not null)
            {
                if (!triggers.TryGetValue(def.TriggerId, out trigger))
                {
                    throw new FsmParseException($"Onbekende trigger '{def.TriggerId}' in transitie '{def.Id}'.");
                }
            }

            effects.TryGetValue(def.Id, out var effect);
            result.Add(new Transition(def.Id, source, destination, trigger, def.Guard, effect));
        }

        return result;
    }

    private static State ResolveState(Dictionary<string, State> states, string id)
    {
        if (!states.TryGetValue(id, out var state))
        {
            throw new FsmParseException($"Onbekende state '{id}'.");
        }

        return state;
    }

    private readonly record struct StateDef(string Id, string ParentId, string Name, string TypeText);

    private readonly record struct TriggerDef(string Id, string Description);

    private readonly record struct ActionDef(string OwnerId, string Description, string TypeText);

    private readonly record struct TransitionDef(string Id, string SourceId, string DestinationId, string? TriggerId, string Guard);
}
