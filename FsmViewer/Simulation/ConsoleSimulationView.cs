using Fsm.Model;

namespace Fsm.Simulation;

public sealed class ConsoleSimulationView : ISimulationView
{
    private readonly TextWriter _output;

    public ConsoleSimulationView(TextWriter output)
    {
        _output = output;
    }

    public void Show(StateMachine fsm)
    {
        _output.WriteLine("Simulation is not implemented yet.");
    }
}
