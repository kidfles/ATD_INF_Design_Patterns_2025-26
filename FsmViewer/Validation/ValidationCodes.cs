namespace Fsm.Validation;

public static class ValidationCodes
{
    public const string NondeterministicTransitions = "FSMV_DETERMINISTIC_SAME_TRIGGER";
    public const string AutomaticTransitionConflict = "FSMV_DETERMINISTIC_AUTOMATIC_WITH_OTHERS";
    public const string InitialStateHasIncoming = "FSMV_INITIAL_HAS_INCOMING";
    public const string FinalStateHasOutgoing = "FSMV_FINAL_HAS_OUTGOING";
    public const string UnreachableState = "FSMV_UNREACHABLE_STATE";
}
