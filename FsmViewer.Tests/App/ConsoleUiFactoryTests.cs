using Fsm.App;
using Fsm.Presentation;
using Fsm.Simulation;

namespace FsmViewer.Tests.App;

public class ConsoleUiFactoryTests
{
    [Fact]
    public void ConsoleFactory_CreatesConsoleFamily()
    {
        var output = new StringWriter();
        var factory = new ConsoleUiFactory(new StringReader(""), output);
        var machine = new Fsm.Parsing.FsmFileParser().Parse(Samples.LampSource);

        Assert.IsType<ConsoleRenderer>(factory.CreateRenderer(machine, output));
        Assert.IsType<ConsoleUserInterface>(factory.CreateUserInterface());
        Assert.IsType<ConsoleSimulationView>(factory.CreateSimulationView());
        Assert.IsType<DiagramView>(factory.CreateDiagramView(machine));
    }
}
