using Fsm.Model;
using Fsm.Validation;

namespace Fsm.Presentation;

public interface IUserInterface
{
    void ShowDiagram(StateMachine fsm);
    void ShowErrors(IEnumerable<ValidationError> errors);
    string Prompt(string message);
    int ChooseFromMenu(string title, IReadOnlyList<string> options);
}
