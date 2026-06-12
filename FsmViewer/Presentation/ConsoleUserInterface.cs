using Fsm.App;
using Fsm.Model;
using Fsm.Validation;

namespace Fsm.Presentation;

public sealed class ConsoleUserInterface : IUserInterface
{
    private readonly IUiFactory _factory;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleUserInterface(IUiFactory factory, TextReader input, TextWriter output)
    {
        _factory = factory;
        _input = input;
        _output = output;
    }

    public void ShowDiagram(StateMachine fsm)
    {
        _factory.CreateDiagramView(fsm).RenderAll();
    }

    public void ShowErrors(IEnumerable<ValidationError> errors)
    {
        _output.WriteLine("Validation errors:");
        foreach (var error in errors)
        {
            _output.WriteLine($"{error.Code}: {error.Message}");
            if (error.OffendingIds.Count > 0)
            {
                _output.WriteLine($"  Offending ids: {string.Join(", ", error.OffendingIds)}");
            }
        }
    }

    public string Prompt(string message)
    {
        _output.Write(message);
        return _input.ReadLine() ?? "";
    }

    public int ChooseFromMenu(string title, IReadOnlyList<string> options)
    {
        while (true)
        {
            _output.WriteLine(title);
            for (var i = 0; i < options.Count; i++)
            {
                _output.WriteLine($"{i + 1}. {options[i]}");
            }

            _output.Write("> ");
            var input = _input.ReadLine();
            if (int.TryParse(input, out var choice) && choice >= 1 && choice <= options.Count)
            {
                return choice - 1;
            }

            _output.WriteLine("Invalid choice.");
        }
    }
}
