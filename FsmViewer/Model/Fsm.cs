namespace Fsm.Model;

public sealed class Fsm
{
    private readonly Dictionary<string, List<Transition>> _outgoing = new();
    private readonly Dictionary<string, List<Transition>> _incoming = new();

    public Fsm(
        IReadOnlyList<State> topLevelStates,
        IReadOnlyList<Transition> transitions,
        IReadOnlyList<Trigger> triggers)
    {
        TopLevelStates = topLevelStates;
        Transitions = transitions;
        Triggers = triggers;

        foreach (var transition in transitions)
        {
            Index(_outgoing, transition.Source.Id, transition);
            Index(_incoming, transition.Destination.Id, transition);
        }
    }

    public IReadOnlyList<State> TopLevelStates { get; }
    public IReadOnlyList<Transition> Transitions { get; }
    public IReadOnlyList<Trigger> Triggers { get; }

    public IReadOnlyList<Transition> TransitionsFrom(State state) =>
        _outgoing.TryGetValue(state.Id, out var list) ? list : Array.Empty<Transition>();

    public IReadOnlyList<Transition> TransitionsTo(State state) =>
        _incoming.TryGetValue(state.Id, out var list) ? list : Array.Empty<Transition>();

    public IEnumerable<State> AllStates()
    {
        foreach (var state in TopLevelStates)
        {
            foreach (var nested in Flatten(state))
            {
                yield return nested;
            }
        }
    }

    private static void Index(Dictionary<string, List<Transition>> index, string key, Transition transition)
    {
        if (!index.TryGetValue(key, out var list))
        {
            list = new List<Transition>();
            index[key] = list;
        }

        list.Add(transition);
    }

    private static IEnumerable<State> Flatten(State state)
    {
        yield return state;

        if (state is CompoundState compound)
        {
            foreach (var child in compound.Substates)
            {
                foreach (var nested in Flatten(child))
                {
                    yield return nested;
                }
            }
        }
    }
}
