using Fsm.Parsing;
using Fsm.Presentation;

namespace FsmViewer.Tests;

public class ConsoleRendererTests
{
    [Fact]
    public void RenderAll_ContainsKeyLines()
    {
        var machine = new FsmFileParser().Parse(Samples.LampSource);
        var output = new StringWriter();
        var view = new DiagramView(machine, new ConsoleRenderer(machine, output));

        view.RenderAll();
        var text = output.ToString();

        Assert.Contains("==== powered ====", text);
        Assert.Contains("[ on ]", text);
        Assert.Contains("entry / light on", text);
        Assert.Contains("---push the switch [time off > 10s] / reset off timer---> off", text);
        Assert.Contains("(O) done", text);
    }

    [Fact]
    public void RenderState_ShowsOnlyOwnTransitions()
    {
        var machine = new FsmFileParser().Parse(Samples.LampSource);
        var off = machine.AllStates().First(s => s.Id == "off");
        var output = new StringWriter();
        var view = new DiagramView(machine, new ConsoleRenderer(machine, output));

        view.RenderState(off);
        var text = output.ToString();

        Assert.Contains("[ off ]", text);
        Assert.Contains("---push the switch---> on", text);
        Assert.DoesNotContain("==== powered ====", text);
        Assert.DoesNotContain("[ on ]", text);
        Assert.DoesNotContain("(O) done", text);
    }
}
