using System.Text;
using Fsm.Model;

namespace Fsm.Presentation;

public sealed class ConsoleRenderer : IFsmVisitor
{
    private readonly StateMachine _fsm;
    private readonly TextWriter _output;
    private int _depth;

    public ConsoleRenderer(StateMachine fsm, TextWriter output)
    {
        _fsm = fsm;
        _output = output;
    }

    public void Visit(InitialState state)
    {
        WriteLine($"O {state.Name}");
        RenderOutgoing(state);
    }

    public void Visit(SimpleState state)
    {
        WriteLine($"[ {state.Name} ]");
        RenderActions(state);
        RenderOutgoing(state);
    }

    public void Visit(CompoundState state)
    {
        WriteLine($"==== {state.Name} ====");
        RenderActions(state);

        _depth++;
        foreach (var substate in state.Substates)
        {
            substate.Accept(this);
        }

        _depth--;

        WriteLine("====");
        RenderOutgoing(state);
    }

    public void Visit(FinalState state)
    {
        WriteLine($"(O) {state.Name}");
        RenderOutgoing(state);
    }

    public void Visit(Transition transition)
    {
        WriteLine($"{transition.Source.Name} {FormatTransition(transition)}");
    }

    private void RenderActions(State state)
    {
        foreach (var action in state.Actions)
        {
            WriteChildLine($"{ActionLabel(action.Kind)} / {action.Description}");
        }
    }

    private void RenderOutgoing(State state)
    {
        foreach (var transition in _fsm.TransitionsFrom(state))
        {
            WriteChildLine(FormatTransition(transition));
        }
    }

    private static string FormatTransition(Transition transition)
    {
        var label = new StringBuilder();

        if (transition.Trigger is not null)
        {
            label.Append(transition.Trigger.Description);
        }

        if (transition.HasGuard)
        {
            if (label.Length > 0)
            {
                label.Append(' ');
            }

            label.Append('[').Append(transition.Guard).Append(']');
        }

        if (transition.Effect is not null)
        {
            if (label.Length > 0)
            {
                label.Append(' ');
            }

            label.Append("/ ").Append(transition.Effect.Description);
        }

        return $"---{label}---> {transition.Destination.Name}";
    }

    private static string ActionLabel(StateActionKind kind) => kind switch
    {
        StateActionKind.Entry => "entry",
        StateActionKind.Do => "do",
        StateActionKind.Exit => "exit",
        _ => kind.ToString()
    };

    private void WriteLine(string text) => _output.WriteLine(Pad(_depth) + text);

    private void WriteChildLine(string text) => _output.WriteLine(Pad(_depth + 1) + text);

    private static string Pad(int depth) => new string(' ', depth * 2);
}
