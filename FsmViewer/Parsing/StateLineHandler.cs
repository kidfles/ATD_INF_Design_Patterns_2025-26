namespace Fsm.Parsing;

internal sealed class StateLineHandler : ILineHandler
{
    public void Handle(IReadOnlyList<Token> tokens, IFsmBuilder builder)
    {
        if (tokens.Count != 5 || !tokens[3].Quoted)
        {
            throw new FsmParseException("Ongeldige STATE-definitie.");
        }

        builder.AddStateDefinition(tokens[1].Text, tokens[2].Text, tokens[3].Text, tokens[4].Text);
    }
}
