using Fsm.Parsing;
using Fsm.Validation;

namespace FsmViewer.Tests.Validation;

public class InitialFinalTransitionValidatorTests
{
    [Fact]
    public void Validate_FindsInitialIncomingAndFinalOutgoingInOnePass()
    {
        var machine = new FsmFileParser().Parse("""
            STATE init _ "start" INITIAL;
            STATE work _ "work" SIMPLE;
            STATE done _ "done" FINAL;
            TRIGGER reset "reset";
            TRANSITION t0 init work;
            TRANSITION t1 work init reset;
            TRANSITION t2 done work reset;
            """);

        var errors = new InitialFinalTransitionValidator().Validate(machine).ToArray();

        Assert.Equal(
            new[] { ValidationCodes.InitialStateHasIncoming, ValidationCodes.FinalStateHasOutgoing },
            errors.Select(error => error.Code));
    }
}
