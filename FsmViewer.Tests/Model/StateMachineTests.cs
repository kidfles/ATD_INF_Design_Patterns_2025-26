using Fsm.Model;

namespace FsmViewer.Tests;

public class StateMachineTests
{
    [Fact]
    public void HandBuiltLamp_NestsCompositeAndIndexesEdges()
    {
        var init = new InitialState("init", "start");
        var powered = new CompoundState("powered", "powered");
        var off = new SimpleState("off", "off");
        var on = new SimpleState("on", "on");
        var done = new FinalState("done", "done");

        powered.AddSubstate(off);
        powered.AddSubstate(on);
        on.AddAction(new StateAction(StateActionKind.Entry, "light on"));

        var push = new Trigger("push_switch", "push the switch");
        var turnOff = new Trigger("turn_off", "turn off");

        var t0 = new Transition("t0", init, off, null, "", null);
        var t1 = new Transition("t1", off, on, push, "", null);
        var t2 = new Transition("t2", on, off, push, "time off > 10s", new Effect("reset off timer"));
        var t3 = new Transition("t3", on, done, turnOff, "", null);

        var machine = new StateMachine(
            new State[] { init, powered, done },
            new[] { t0, t1, t2, t3 },
            new[] { push, turnOff });

        Assert.Equal(3, machine.TopLevelStates.Count);
        Assert.Equal(new State[] { off, on }, powered.Substates);
        Assert.Same(powered, off.Parent);
        Assert.Same(powered, on.Parent);

        Assert.Equal(new[] { t1 }, machine.TransitionsFrom(off));
        Assert.Equal(new[] { t0, t2 }, machine.TransitionsTo(off));
        Assert.Equal(new[] { t2, t3 }, machine.TransitionsFrom(on));

        Assert.Equal(
            new State[] { init, powered, off, on, done },
            machine.AllStates().ToList());

        Assert.False(t2.IsAutomatic);
        Assert.True(t2.HasGuard);
        Assert.Equal("push_switch", t2.Trigger!.Id);
        Assert.Equal("time off > 10s", t2.Guard);
        Assert.Equal("reset off timer", t2.Effect!.Description);
    }
}
