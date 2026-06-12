using Fsm.Model;
using Fsm.Validation;

namespace FsmViewer.Tests.Validation;

public class ValidationRunnerTests
{
    [Fact]
    public void Validate_AggregatesAllValidators()
    {
        var machine = EmptyMachine();
        var runner = new ValidationRunner(new IValidator[]
        {
            new StubValidator("A"),
            new StubValidator("B")
        });

        var errors = runner.Validate(machine);

        Assert.Equal(new[] { "A", "B" }, errors.Select(error => error.Code));
    }

    [Fact]
    public void DefaultConstructor_WiresTheThreeValidators()
    {
        var validators = ValidationRunner.CreateDefaultValidators();

        Assert.Collection(
            validators,
            validator => Assert.IsType<DeterministicTransitionsValidator>(validator),
            validator => Assert.IsType<InitialFinalTransitionValidator>(validator),
            validator => Assert.IsType<ReachabilityValidator>(validator));
    }

    private static StateMachine EmptyMachine() =>
        new(Array.Empty<State>(), Array.Empty<Transition>(), Array.Empty<Trigger>());

    private sealed class StubValidator : IValidator
    {
        private readonly string _code;

        public StubValidator(string code)
        {
            _code = code;
        }

        public IEnumerable<ValidationError> Validate(StateMachine fsm)
        {
            yield return new ValidationError(_code, _code, Array.Empty<string>());
        }
    }
}
