using Fsm.App;

namespace FsmViewer.Tests.App;

public class ProgramPipelineTests
{
    [Fact]
    public void Run_WithInvalidFsm_ShowsErrorsAndStopsBeforeMenu()
    {
        var path = WriteTempFsm("""
            STATE init _ "start" INITIAL;
            STATE open _ "open" SIMPLE;
            STATE closed _ "closed" SIMPLE;
            STATE locked _ "locked" SIMPLE;
            TRIGGER push "push";
            TRANSITION t0 init open;
            TRANSITION t1 open closed push;
            TRANSITION t2 open locked push;
            """);
        var output = new StringWriter();
        var factory = new ConsoleUiFactory(new StringReader(""), output);

        var exitCode = Program.Run(new[] { path }, factory);

        var text = output.ToString();
        Assert.Equal(1, exitCode);
        Assert.Contains("FSMV_DETERMINISTIC_SAME_TRIGGER", text);
        Assert.DoesNotContain("Menu", text);
    }

    [Fact]
    public void Run_WithValidFsm_ShowsDiagramThenMenu()
    {
        var path = WriteTempFsm(Samples.LampSource);
        var output = new StringWriter();
        var factory = new ConsoleUiFactory(new StringReader("4"), output);

        var exitCode = Program.Run(new[] { path }, factory);

        var text = output.ToString();
        Assert.Equal(0, exitCode);
        Assert.Contains("==== powered ====", text);
        Assert.Contains("1. Render all", text);
        Assert.Contains("2. Render part", text);
        Assert.Contains("3. Simulate", text);
    }

    private static string WriteTempFsm(string source)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.fsm");
        File.WriteAllText(path, source);
        return path;
    }
}
