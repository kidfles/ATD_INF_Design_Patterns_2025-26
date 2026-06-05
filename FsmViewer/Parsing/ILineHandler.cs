namespace Fsm.Parsing;

internal interface ILineHandler
{
    void Handle(IReadOnlyList<Token> tokens, IFsmBuilder builder);
}
