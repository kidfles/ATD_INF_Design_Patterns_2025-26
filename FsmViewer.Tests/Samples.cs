namespace FsmViewer.Tests;

internal static class Samples
{
    public const string LampSource = """
        STATE init _ "start" INITIAL;
        STATE powered _ "powered" COMPOUND;
        STATE off powered "off" SIMPLE;
        STATE on powered "on" SIMPLE;
        STATE done _ "done" FINAL;
        TRIGGER push_switch "push the switch";
        TRIGGER turn_off "turn off";
        ACTION on "light on" ENTRY;
        TRANSITION t0 init off;
        TRANSITION t1 off on push_switch;
        TRANSITION t2 on off push_switch "time off > 10s";
        TRANSITION t3 on done turn_off;
        ACTION t2 "reset off timer" TRANSITION_ACTION;
        """;

    public const string NestedAccountSource = """
        STATE created _ "Created" COMPOUND;
        STATE active created "Active" COMPOUND;
        STATE inactive created "Inactive" SIMPLE;
        STATE loggedIn active "LoggedIn" SIMPLE;
        STATE loggedOut active "LoggedOut" SIMPLE;
        """;
}
