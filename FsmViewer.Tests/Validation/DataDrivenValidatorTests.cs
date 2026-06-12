using Fsm.Parsing;
using Fsm.Validation;

namespace FsmViewer.Tests.Validation;

public class DataDrivenValidatorTests
{
    public static IEnumerable<object[]> InvalidFiles()
    {
        yield return new object[] { "TestData/Invalid/same-trigger-no-guards.fsm", ValidationCodes.NondeterministicTransitions };
        yield return new object[] { "TestData/Invalid/automatic-with-other.fsm", ValidationCodes.AutomaticTransitionConflict };
        yield return new object[] { "TestData/Invalid/initial-incoming.fsm", ValidationCodes.InitialStateHasIncoming };
        yield return new object[] { "TestData/Invalid/final-outgoing.fsm", ValidationCodes.FinalStateHasOutgoing };
        yield return new object[] { "TestData/Invalid/unreachable-state.fsm", ValidationCodes.UnreachableState };
    }

    public static IEnumerable<object[]> ValidFiles()
    {
        yield return new object[] { "TestData/Valid/deterministic-guarded.fsm" };
        yield return new object[] { "TestData/Valid/self-transitions.fsm" };
    }

    [Theory]
    [MemberData(nameof(InvalidFiles))]
    public void InvalidFiles_TriggerExactlyExpectedValidatorCode(string relativePath, string expectedCode)
    {
        var machine = ParseFile(relativePath);

        var errors = new ValidationRunner().Validate(machine);

        var error = Assert.Single(errors);
        Assert.Equal(expectedCode, error.Code);
    }

    [Theory]
    [MemberData(nameof(ValidFiles))]
    public void ValidFiles_AreClean(string relativePath)
    {
        var machine = ParseFile(relativePath);

        var errors = new ValidationRunner().Validate(machine);

        Assert.Empty(errors);
    }

    private static Fsm.Model.StateMachine ParseFile(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, relativePath);
        return new FsmFileParser().Parse(File.ReadAllText(path));
    }
}
