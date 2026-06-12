using Fsm.Model;

namespace Fsm.Validation;

public interface IValidator
{
    IEnumerable<ValidationError> Validate(StateMachine fsm);
}
