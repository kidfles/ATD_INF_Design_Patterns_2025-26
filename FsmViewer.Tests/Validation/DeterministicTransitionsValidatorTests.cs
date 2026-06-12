using Fsm.Parsing;
using Fsm.Validation;

namespace FsmViewer.Tests.Validation;

public class DeterministicTransitionsValidatorTests
{
    [Fact]
    public void SameTriggerWithoutGuards_IsInvalid()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE open _ "open" SIMPLE;
            STATE closed _ "closed" SIMPLE;
            STATE locked _ "locked" SIMPLE;
            TRIGGER push "push";
            TRANSITION t0 init open;
            TRANSITION t1 open closed push;
            TRANSITION t2 open locked push;
            """);

        var error = Assert.Single(new DeterministicTransitionsValidator().Validate(machine));

        Assert.Equal(ValidationCodes.NondeterministicTransitions, error.Code);
        Assert.Equal(new[] { "t1", "t2" }, error.OffendingIds);
    }

    [Fact]
    public void SameTriggerWithDistinctGuards_IsValid()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE open _ "open" SIMPLE;
            STATE closed _ "closed" SIMPLE;
            STATE locked _ "locked" SIMPLE;
            TRIGGER push "push";
            TRANSITION t0 init open;
            TRANSITION t1 open closed push "isClosed";
            TRANSITION t2 open locked push "isLocked";
            """);

        var errors = new DeterministicTransitionsValidator().Validate(machine);

        Assert.Empty(errors);
    }

    [Fact]
    public void AutomaticTransitionAlongsideAnotherOutgoing_IsInvalid()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE open _ "open" SIMPLE;
            STATE closed _ "closed" SIMPLE;
            STATE locked _ "locked" SIMPLE;
            TRIGGER push "push";
            TRANSITION t0 init open;
            TRANSITION t1 open closed;
            TRANSITION t2 open locked push;
            """);

        var error = Assert.Single(new DeterministicTransitionsValidator().Validate(machine));

        Assert.Equal(ValidationCodes.AutomaticTransitionConflict, error.Code);
        Assert.Equal(new[] { "t1", "t2" }, error.OffendingIds);
    }

    [Fact]
    public void SelfTransitions_DoNotTripDeterminismRules()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE active _ "active" SIMPLE;
            STATE done _ "done" FINAL;
            TRIGGER tick "tick";
            TRANSITION t0 init active;
            TRANSITION t1 active active tick;
            TRANSITION t2 active active;
            TRANSITION t3 active done tick;
            """);

        var errors = new DeterministicTransitionsValidator().Validate(machine);

        Assert.Empty(errors);
    }
}
