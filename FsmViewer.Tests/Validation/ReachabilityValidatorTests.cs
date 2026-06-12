using Fsm.Parsing;
using Fsm.Validation;

namespace FsmViewer.Tests.Validation;

public class ReachabilityValidatorTests
{
    [Fact]
    public void Validate_FindsUnreachableStates()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE active _ "active" SIMPLE;
            STATE orphan _ "orphan" SIMPLE;
            TRANSITION t0 init active;
            """);

        var error = Assert.Single(new ReachabilityValidator().Validate(machine));

        Assert.Equal(ValidationCodes.UnreachableState, error.Code);
        Assert.Equal(new[] { "orphan" }, error.OffendingIds);
    }

    [Fact]
    public void Validate_CountsAncestorCompoundsAsReached()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE container _ "container" COMPOUND;
            STATE active container "active" SIMPLE;
            TRANSITION t0 init active;
            """);

        var errors = new ReachabilityValidator().Validate(machine);

        Assert.Empty(errors);
    }
}
