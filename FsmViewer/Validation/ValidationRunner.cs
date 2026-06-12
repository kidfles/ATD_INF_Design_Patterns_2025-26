using Fsm.Model;

namespace Fsm.Validation;

public sealed class ValidationRunner
{
    private readonly IReadOnlyList<IValidator> _validators;

    public ValidationRunner()
        : this(CreateDefaultValidators())
    {
    }

    public ValidationRunner(IEnumerable<IValidator> validators)
    {
        _validators = validators.ToArray();
    }

    public IReadOnlyList<ValidationError> Validate(StateMachine fsm) =>
        _validators.SelectMany(validator => validator.Validate(fsm)).ToArray();

    public static IReadOnlyList<IValidator> CreateDefaultValidators() =>
        new IValidator[]
        {
            new DeterministicTransitionsValidator(),
            new InitialFinalTransitionValidator(),
            new ReachabilityValidator()
        };
}
