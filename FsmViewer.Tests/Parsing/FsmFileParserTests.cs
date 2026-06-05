using Fsm.Model;
using Fsm.Parsing;

namespace FsmViewer.Tests;

public class FsmFileParserTests
{
    [Fact]
    public void ParsesLamp_IntoExpectedStructure()
    {
        var machine = new FsmFileParser().Parse(Samples.LampSource);

        Assert.Equal(3, machine.TopLevelStates.Count);

        var powered = (CompoundState)machine.AllStates().First(s => s.Id == "powered");
        Assert.Equal(2, powered.Substates.Count);

        var off = machine.AllStates().First(s => s.Id == "off");
        var on = machine.AllStates().First(s => s.Id == "on");
        Assert.Same(powered, off.Parent);
        Assert.Same(powered, on.Parent);

        var t2 = machine.Transitions.First(t => t.Id == "t2");
        Assert.Equal("push_switch", t2.Trigger!.Id);
        Assert.Equal("time off > 10s", t2.Guard);
        Assert.Equal("reset off timer", t2.Effect!.Description);

        Assert.Equal("t1", machine.TransitionsFrom(off).Single().Id);
        Assert.Equal(new[] { "t0", "t2" }, machine.TransitionsTo(off).Select(t => t.Id));
    }

    [Fact]
    public void ParsesNestedAccount_WithDepthGreaterThanOne()
    {
        var machine = new FsmFileParser().Parse(Samples.NestedAccountSource);

        var loggedIn = machine.AllStates().First(s => s.Id == "loggedIn");

        Assert.NotNull(loggedIn.Parent);
        Assert.Equal("active", loggedIn.Parent!.Id);
        Assert.NotNull(loggedIn.Parent!.Parent);
        Assert.Equal("created", loggedIn.Parent!.Parent!.Id);
    }
}
