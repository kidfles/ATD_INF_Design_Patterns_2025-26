using Fsm.App;
using Fsm.Presentation;
using Fsm.Validation;

namespace FsmViewer.Tests.Presentation;

public class ConsoleUserInterfaceTests
{
    [Fact]
    public void Prompt_ReadsFromConfiguredInput()
    {
        var input = new StringReader("state1");
        var output = new StringWriter();
        IUserInterface ui = new ConsoleUiFactory(input, output).CreateUserInterface();

        var result = ui.Prompt("id: ");

        Assert.Equal("state1", result);
        Assert.Equal("id: ", output.ToString());
    }

    [Fact]
    public void ChooseFromMenu_ReturnsZeroBasedChoice()
    {
        var input = new StringReader("2");
        var output = new StringWriter();
        IUserInterface ui = new ConsoleUiFactory(input, output).CreateUserInterface();

        var result = ui.ChooseFromMenu("Menu", new[] { "One", "Two" });

        Assert.Equal(1, result);
        Assert.Contains("1. One", output.ToString());
        Assert.Contains("2. Two", output.ToString());
    }

    [Fact]
    public void ShowErrors_PrintsCodesAndOffendingIds()
    {
        var output = new StringWriter();
        IUserInterface ui = new ConsoleUiFactory(new StringReader(""), output).CreateUserInterface();

        ui.ShowErrors(new[]
        {
            new ValidationError("CODE", "Message", new[] { "s1", "t1" })
        });

        var text = output.ToString();
        Assert.Contains("CODE: Message", text);
        Assert.Contains("s1, t1", text);
    }
}
