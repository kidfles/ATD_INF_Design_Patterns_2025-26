using Fsm.Model;
using Fsm.Parsing;
using Fsm.Presentation;
using Fsm.Validation;

namespace Fsm.App;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.ExitCode = Run(args, new ConsoleUiFactory());
    }

    internal static int Run(string[] args, IUiFactory uiFactory)
    {
        var ui = uiFactory.CreateUserInterface();
        if (args.Length == 0)
        {
            ui.ShowErrors(new[]
            {
                new ValidationError(
                    "FSMV_APP_MISSING_PATH",
                    "Provide a path to an FSM file.",
                    Array.Empty<string>())
            });
            return 1;
        }

        StateMachine fsm;
        try
        {
            fsm = new FsmFileParser().ParseFile(args[0]);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FsmParseException)
        {
            ui.ShowErrors(new[]
            {
                new ValidationError(
                    "FSMV_APP_LOAD_FAILED",
                    ex.Message,
                    new[] { args[0] })
            });
            return 1;
        }

        var errors = new ValidationRunner().Validate(fsm);
        if (errors.Count > 0)
        {
            ui.ShowErrors(errors);
            return 1;
        }

        ui.ShowDiagram(fsm);
        RunMenu(fsm, ui, uiFactory);
        return 0;
    }

    private static void RunMenu(StateMachine fsm, IUserInterface ui, IUiFactory uiFactory)
    {
        var options = new[] { "Render all", "Render part", "Simulate", "Exit" };
        while (true)
        {
            var choice = ui.ChooseFromMenu("Menu", options);
            switch (choice)
            {
                case 0:
                    ui.ShowDiagram(fsm);
                    break;
                case 1:
                    RenderPart(fsm, ui, uiFactory);
                    break;
                case 2:
                    uiFactory.CreateSimulationView().Show(fsm);
                    break;
                default:
                    return;
            }
        }
    }

    private static void RenderPart(StateMachine fsm, IUserInterface ui, IUiFactory uiFactory)
    {
        var id = ui.Prompt("State or transition id: ");
        var view = uiFactory.CreateDiagramView(fsm);

        var state = fsm.AllStates().FirstOrDefault(candidate => candidate.Id == id);
        if (state is CompoundState compound)
        {
            view.RenderCompound(compound);
            return;
        }

        if (state is not null)
        {
            view.RenderState(state);
            return;
        }

        var transition = fsm.Transitions.FirstOrDefault(candidate => candidate.Id == id);
        if (transition is not null)
        {
            view.RenderTransition(transition);
            return;
        }

        ui.ShowErrors(new[]
        {
            new ValidationError(
                "FSMV_APP_UNKNOWN_ID",
                $"Unknown state or transition id '{id}'.",
                new[] { id })
        });
    }
}
