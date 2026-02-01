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
                "Set how many seconds per mile, default is 5. Decimal numbers are allowed."
            },
            {
                "set day_transition_duration <value>",
                "Set the day transition time in campaign mode, default is 5 seconds, integer only."
            },
            {
                "set interval <value>",
                "Set the interval in campaign mode. Interval determines the hour and days. Each day has 12 intervals, e.g. interval 15 = day 2 06:00"
            },
            { "set temperature <value>", "Set the current temperature in campaign mode." },
            {
                "set max_days <value>",
                "Set the max days in campaign mode, i.e., how many days for the player to reach to win."
            },
        };

    public override void Execute(string[] args)
    {
        if (args.Length < 2)
            throw new Exception("Missing arguments.");

        switch (args[0])
        {
            case "seconds_per_mile":
            {
                if (!float.TryParse(args[1], out float value))
                    throw new Exception($"\"{args[1]}\" is not a valid number.");
                CityModeManager.Instance.SecondsPerMile = value;
                ConsoleManager.Instance.Output(
                    $"Seconds per mile has been set to {args[1]}. It will take effect on the next interval."
                );
                break;
            }
            case "day_transition_duration":
            {
                if (!int.TryParse(args[1], out int value))
                    throw new Exception($"\"{args[1]}\" is not a valid integer.");
                CampaignModeManager.Instance.DayTransitionDuration = value;
                ConsoleManager.Instance.Output(
                    $"Day transition duration has been set to {args[1]}. It will take effect on the next transition"
                );
                break;
            }
            case "interval":
            {
                if (!int.TryParse(args[1], out int value))
                    throw new Exception($"\"{args[1]}\" is not a valid integer.");
                CampaignModeManager.Instance.Interval = value;
                ConsoleManager.Instance.Output($"Interval has been set to {args[1]}.");
                break;
            }
            case "temperature":
            {
                if (!int.TryParse(args[1], out int value))
                    throw new Exception($"\"{args[1]}\" is not a valid integer.");
                CampaignModeManager.Instance.Temperature = value;
                ConsoleManager.Instance.Output($"Temperature has been set to {args[1]}.");
                break;
            }
            default:
                throw new Exception($"Invalid argument \"{args[0]}\"");
        }
    }
}
