using Fsm.Model;

namespace Fsm.Validation;

public sealed class DeterministicTransitionsValidator : IValidator
{
    public IEnumerable<ValidationError> Validate(StateMachine fsm)
    {
        foreach (var state in fsm.AllStates())
        {
            var outgoing = fsm.TransitionsFrom(state)
                .Where(transition => !transition.IsSelfTransition)
                .ToArray();

            foreach (var error in ValidateSameTriggerTransitions(state, outgoing))
            {
                yield return error;
            }

            foreach (var error in ValidateAutomaticTransitions(state, outgoing))
            {
                yield return error;
            }
        }
    }

    private static IEnumerable<ValidationError> ValidateSameTriggerTransitions(
        State state,
        IReadOnlyList<Transition> outgoing)
    {
        var groups = outgoing
            .Where(transition => !transition.IsAutomatic)
            .GroupBy(transition => transition.Trigger!.Id);

        foreach (var group in groups)
        {
            var transitions = group.ToArray();
            if (transitions.Length < 2)
            {
                continue;
            }

            var hasUnguardedTransition = transitions.Any(transition => !transition.HasGuard);
            var hasDuplicateGuard = transitions
                .Where(transition => transition.HasGuard)
                .GroupBy(transition => transition.Guard, StringComparer.Ordinal)
                .Any(guardGroup => guardGroup.Count() > 1);

            if (!hasUnguardedTransition && !hasDuplicateGuard)
            {
                continue;
            }

            yield return new ValidationError(
                ValidationCodes.NondeterministicTransitions,
                $"State '{state.Id}' has same-trigger transitions that are not distinguished by guards.",
                transitions.Select(transition => transition.Id).ToArray());
        }
    }

    private static IEnumerable<ValidationError> ValidateAutomaticTransitions(
        State state,
        IReadOnlyList<Transition> outgoing)
    {
        if (outgoing.Count < 2)
        {
            yield break;
        }

        foreach (var automatic in outgoing.Where(transition => transition.IsAutomatic))
        {
            yield return new ValidationError(
                ValidationCodes.AutomaticTransitionConflict,
                $"State '{state.Id}' has an automatic transition alongside another outgoing transition.",
                outgoing.Select(transition => transition.Id).ToArray());
        }
    }
}
