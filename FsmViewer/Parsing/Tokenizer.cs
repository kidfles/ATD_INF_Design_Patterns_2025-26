using System.Text;

namespace Fsm.Parsing;

internal static class Tokenizer
{
    public static IEnumerable<string> SplitDefinitions(string source)
    {
        var buffer = new StringBuilder();
        var inQuotes = false;
        var i = 0;

        while (i < source.Length)
        {
            var c = source[i];

            if (inQuotes)
            {
                buffer.Append(c);
                if (c == '"')
                {
                    inQuotes = false;
                }

                i++;
                continue;
            }

            switch (c)
            {
                case '"':
                    inQuotes = true;
                    buffer.Append(c);
                    i++;
                    break;
                case '#':
                    while (i < source.Length && source[i] != '\n')
                    {
                        i++;
                    }

                    break;
                case ';':
                    yield return buffer.ToString();
                    buffer.Clear();
                    i++;
                    break;
                default:
                    buffer.Append(c);
                    i++;
                    break;
            }
        }

        foreach (var ch in buffer.ToString())
        {
            if (!char.IsWhiteSpace(ch))
            {
                throw new FsmParseException("Definitie zonder afsluitende ';'.");
            }
        }
    }

    public static IReadOnlyList<Token> Tokenize(string definition)
    {
        var tokens = new List<Token>();
        var buffer = new StringBuilder();
        var i = 0;

        while (i < definition.Length)
        {
            var c = definition[i];

            if (c == '"')
            {
                FlushBare(tokens, buffer);
                var content = new StringBuilder();
                i++;
                while (i < definition.Length && definition[i] != '"')
                {
                    content.Append(definition[i]);
                    i++;
                }

                i++;
                tokens.Add(new Token(content.ToString(), true));
                continue;
            }

            if (char.IsWhiteSpace(c) || c == ':')
            {
                FlushBare(tokens, buffer);
                i++;
                continue;
            }

            if (c == '-' && i + 1 < definition.Length && definition[i + 1] == '>')
            {
                FlushBare(tokens, buffer);
                i += 2;
                continue;
            }

            buffer.Append(c);
            i++;
        }

        FlushBare(tokens, buffer);
        return tokens;
    }

    private static void FlushBare(List<Token> tokens, StringBuilder buffer)
    {
        if (buffer.Length == 0)
        {
            return;
        }

        tokens.Add(new Token(buffer.ToString(), false));
        buffer.Clear();
    }
}
