using System;
using System.Collections.Generic;

public class SetCommand : Command
{
    public override string Id => "set";

    public override string HelpText => "Set a variable to a value.";

    public override Dictionary<string, string> Usage =>
        new()
        {
            {
                "set seconds_per_mile <value>",
                "Set how many seconds per mile, default is 5. Decimal numbers are allowed. This will also affect the 5 mins timer for documentation mission."
            },
        };

    public override void Execute(string[] args)
    {
        if (args.Length < 2)
            throw new Exception("Missing arguments.");

        switch (args[0])
        {
            case "seconds_per_mile":
                if (!float.TryParse(args[1], out float value))
                    throw new Exception($"\"{args[1]}\" is not a valid number.");
                GameManager.Instance.SecondsPerMile = value;
                ConsoleManager.Instance.Output(
                    $"Seconds per mile has been set to {args[1]}. It will take effect on the next interval."
                );
                break;
            default:
                throw new Exception($"Invalid argument \"{args[0]}\"");
        }
    }
}
