namespace Fsm.Parsing;

internal readonly struct Token
{
    public Token(string text, bool quoted)
    {
        Text = text;
        Quoted = quoted;
    }

    public string Text { get; }
    public bool Quoted { get; }
}
