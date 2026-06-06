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
                "set day <value>",
                "Set the day in campaign mode (1-indexed). Preserves current time of day."
            },
            {
                "set hours <value>",
                "Set the hour in campaign mode. Valid values: 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22. Preserves current day."
            },
            { "set temperature <value>", "Set the current temperature in campaign mode." },
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
            case "day":
            {
                if (!int.TryParse(args[1], out int value) || value < 1)
                    throw new Exception(
                        $"\"{args[1]}\" is not a valid day (must be an integer >= 1)."
                    );
                int currentInterval = CampaignModeManager.Instance.Interval;
                CampaignModeManager.Instance.Interval = (value - 1) * 12 + (currentInterval % 12);
                ConsoleManager.Instance.Output($"Day has been set to {value}.");
                break;
            }
            case "hours":
            {
                if (
                    !int.TryParse(args[1], out int value)
                    || value < 0
                    || value > 22
                    || value % 2 != 0
                )
                    throw new Exception(
                        $"\"{args[1]}\" is not a valid hour (must be 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, or 22)."
                    );
                int currentDay = CampaignModeManager.Instance.Interval / 12;
                CampaignModeManager.Instance.Interval = currentDay * 12 + value / 2;
                ConsoleManager.Instance.Output($"Hours has been set to {value}:00.");
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
