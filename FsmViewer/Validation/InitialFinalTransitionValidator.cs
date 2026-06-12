using Fsm.Model;

namespace Fsm.Validation;

public sealed class InitialFinalTransitionValidator : IValidator
{
    public IEnumerable<ValidationError> Validate(StateMachine fsm)
    {
        foreach (var state in fsm.AllStates())
        {
            if (state is InitialState)
            {
                var incoming = fsm.TransitionsTo(state);
                if (incoming.Count > 0)
                {
                    yield return new ValidationError(
                        ValidationCodes.InitialStateHasIncoming,
                        $"Initial state '{state.Id}' has incoming transitions.",
                        new[] { state.Id }.Concat(incoming.Select(transition => transition.Id)).ToArray());
                }
            }

            if (state is FinalState)
            {
                var outgoing = fsm.TransitionsFrom(state);
                if (outgoing.Count > 0)
                {
                    yield return new ValidationError(
                        ValidationCodes.FinalStateHasOutgoing,
                        $"Final state '{state.Id}' has outgoing transitions.",
                        new[] { state.Id }.Concat(outgoing.Select(transition => transition.Id)).ToArray());
                }
            }
        }
    }
}
