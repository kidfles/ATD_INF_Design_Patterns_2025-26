using Fsm.Model;
using Fsm.Presentation;
using Fsm.Simulation;

namespace Fsm.App;

public sealed class ConsoleUiFactory : IUiFactory
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleUiFactory()
        : this(Console.In, Console.Out)
    {
    }

    public ConsoleUiFactory(TextReader input, TextWriter output)
    {
        _input = input;
        _output = output;
    }

    public IFsmVisitor CreateRenderer(StateMachine fsm, TextWriter output) =>
        new ConsoleRenderer(fsm, output);

    public DiagramView CreateDiagramView(StateMachine fsm) =>
        new(fsm, CreateRenderer(fsm, _output));

    public IUserInterface CreateUserInterface() =>
        new ConsoleUserInterface(this, _input, _output);

    public ISimulationView CreateSimulationView() =>
        new ConsoleSimulationView(_output);
}
