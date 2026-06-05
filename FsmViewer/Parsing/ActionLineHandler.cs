namespace Fsm.Parsing;

internal sealed class ActionLineHandler : ILineHandler
{
    public void Handle(IReadOnlyList<Token> tokens, IFsmBuilder builder)
    {
        if (tokens.Count != 4 || !tokens[2].Quoted)
        {
            throw new FsmParseException("Ongeldige ACTION-definitie.");
        }

        builder.AddActionDefinition(tokens[1].Text, tokens[2].Text, tokens[3].Text);
    }
}
