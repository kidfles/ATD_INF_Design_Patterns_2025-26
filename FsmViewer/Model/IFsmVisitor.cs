namespace Fsm.Model;

public interface IFsmVisitor
{
    void Visit(InitialState state);
    void Visit(SimpleState state);
    void Visit(CompoundState state);
    void Visit(FinalState state);
    void Visit(Transition transition);
}
