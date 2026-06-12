using Fsm.Model;
using Fsm.Presentation;
using Fsm.Simulation;

namespace Fsm.App;

public interface IUiFactory
{
    IFsmVisitor CreateRenderer(StateMachine fsm, TextWriter output);
    DiagramView CreateDiagramView(StateMachine fsm);
    IUserInterface CreateUserInterface();
    ISimulationView CreateSimulationView();
}
