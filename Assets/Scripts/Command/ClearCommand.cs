using System.Collections.Generic;

public class ClearCommand : Command
{
    public override string Id => "clear";

    public override string HelpText => "Clears the console output window";

    public override Dictionary<string, string> Usage => new();

    public override void Execute(string[] args)
    {
        ConsoleManager.Instance.Clear();
    }
}
