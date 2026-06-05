using Fsm.Model;

namespace Fsm.Presentation;

public sealed class DiagramView
{
    private readonly StateMachine _fsm;
    private readonly IFsmVisitor _renderer;

    public DiagramView(StateMachine fsm, IFsmVisitor renderer)
    {
        _fsm = fsm;
        _renderer = renderer;
    }

    public void RenderAll()
    {
        foreach (var state in _fsm.TopLevelStates)
        {
            state.Accept(_renderer);
        }
    }

    public void RenderState(State state)
    {
        state.Accept(_renderer);

        foreach (var transition in _fsm.TransitionsTo(state))
        {
            transition.Accept(_renderer);
        }
    }

    public void RenderCompound(CompoundState compound)
    {
        compound.Accept(_renderer);
    }

    public void RenderTransition(Transition transition)
    {
        transition.Accept(_renderer);
    }
}
