using Fsm.Model;

namespace Fsm.Parsing;

public sealed class FsmFileParser
{
    private readonly Dictionary<string, ILineHandler> _handlers =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["STATE"] = new StateLineHandler(),
            ["TRIGGER"] = new TriggerLineHandler(),
            ["ACTION"] = new ActionLineHandler(),
            ["TRANSITION"] = new TransitionLineHandler()
        };

    public StateMachine ParseFile(string path) => Parse(File.ReadAllText(path));

    public StateMachine Parse(string source) => Parse(source, new FsmObjectBuilder());

    public StateMachine Parse(string source, IFsmBuilder builder)
    {
        foreach (var definition in Tokenizer.SplitDefinitions(source))
        {
            var tokens = Tokenizer.Tokenize(definition);
            if (tokens.Count == 0)
            {
                continue;
            }

            if (!_handlers.TryGetValue(tokens[0].Text, out var handler))
            {
                throw new FsmParseException($"Onbekend sleutelwoord '{tokens[0].Text}'.");
            }

            handler.Handle(tokens, builder);
        }

        return builder.Build();
    }
}
