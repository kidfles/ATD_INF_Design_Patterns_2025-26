namespace Fsm.Parsing;

internal sealed class TriggerLineHandler : ILineHandler
{
    public void Handle(IReadOnlyList<Token> tokens, IFsmBuilder builder)
    {
        if (tokens.Count != 3 || !tokens[2].Quoted)
        {
            throw new FsmParseException("Ongeldige TRIGGER-definitie.");
        }

        builder.AddTriggerDefinition(tokens[1].Text, tokens[2].Text);
    }
}
