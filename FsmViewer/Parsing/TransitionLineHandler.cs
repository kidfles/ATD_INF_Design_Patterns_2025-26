namespace Fsm.Parsing;

internal sealed class TransitionLineHandler : ILineHandler
{
    public void Handle(IReadOnlyList<Token> tokens, IFsmBuilder builder)
    {
        if (tokens.Count < 4)
        {
            throw new FsmParseException("Ongeldige TRANSITION-definitie.");
        }

        string? triggerId = null;
        var guard = "";

        for (var i = 4; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Quoted)
            {
                guard = token.Text;
            }
            else
            {
                triggerId = token.Text;
            }
        }

        builder.AddTransitionDefinition(tokens[1].Text, tokens[2].Text, tokens[3].Text, triggerId, guard);
    }
}
