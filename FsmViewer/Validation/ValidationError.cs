namespace Fsm.Validation;

public sealed record ValidationError(
    string Code,
    string Message,
    IReadOnlyList<string> OffendingIds);
