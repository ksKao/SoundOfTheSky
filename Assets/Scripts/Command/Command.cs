using System.Collections.Generic;

public abstract class Command
{
    public abstract string Id { get; }
    public abstract string HelpText { get; }
    public abstract Dictionary<string, string> Usage { get; }

    public abstract void Execute(string[] args);
}
