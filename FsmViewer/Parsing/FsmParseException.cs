namespace Fsm.Parsing;

public sealed class FsmParseException : Exception
{
    public FsmParseException(string message) : base(message)
    {
    }
}
