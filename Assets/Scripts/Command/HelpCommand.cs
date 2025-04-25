using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HelpCommand : Command
{
    public override string Id => "help";

    public override string HelpText => "Get info about command(s).";

    public override Dictionary<string, string> Usage => new()
    {
        { "help", "Get all commands." },
        { "help <command>", "Get info about the command" }
    };

    public override void Execute(string[] args)
    {
        StringBuilder output = new();

        if (args.Length == 0)
        {
            output.AppendLine("All commands: ");
            foreach (Command command in ConsoleManager.Instance.Commands)
            {
                output.AppendLine($"{command.Id} - {command.HelpText}");
            }
        }
        else
        {
            Command command = ConsoleManager.Instance.Commands.FirstOrDefault(c => c.Id == args[0]) ?? throw new Exception($"\"{args[0]}\" is not a valid command");

            output.AppendLine(command.HelpText);

            // cache here to use multiple times
            Dictionary<string, string> usage = Usage;
            if (usage.Count > 0)
            {
                output.AppendLine("Usage:");

                foreach (KeyValuePair<string, string> kv in usage)
                {
                    output.AppendLine($"{kv.Key}: {kv.Value}");
                }
            }
        }

        ConsoleManager.Instance.Output(output.ToString());
    }
}
