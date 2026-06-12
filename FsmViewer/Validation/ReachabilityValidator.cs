using Fsm.Model;

namespace Fsm.Validation;

public sealed class ReachabilityValidator : IValidator
{
    public IEnumerable<ValidationError> Validate(StateMachine fsm)
    {
        var reached = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<State>();

        foreach (var initial in fsm.AllStates().OfType<InitialState>())
        {
            MarkReached(initial, reached);
            queue.Enqueue(initial);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var transition in fsm.TransitionsFrom(current))
            {
                if (reached.Contains(transition.Destination.Id))
                {
                    continue;
                }

                MarkReached(transition.Destination, reached);
                queue.Enqueue(transition.Destination);
            }
        }

        foreach (var state in fsm.AllStates())
        {
            if (state is InitialState || reached.Contains(state.Id))
            {
                continue;
            }

            yield return new ValidationError(
                ValidationCodes.UnreachableState,
                $"State '{state.Id}' is not reachable from any initial state.",
                new[] { state.Id });
        }
    }

    private static void MarkReached(State state, ISet<string> reached)
    {
        for (State? current = state; current is not null; current = current.Parent)
        {
            reached.Add(current.Id);
        }
    }
}
