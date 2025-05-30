using System;
using System.Collections.Generic;
using System.Linq;

public class UnlockCommand : Command
{
    public override string Id => "unlock";

    public override string HelpText => "Unlocks a train without paying.";

    public override Dictionary<string, string> Usage =>
        new()
        {
            {
                "unlock <train_name>",
                "Unlocks a train by its name (case insensitive). If train name contains space, replace it with underscore."
            },
            { "unlock all", "Unlocks all trains at once" },
        };

    public override void Execute(string[] args)
    {
        if (args.Length < 1)
            throw new Exception("Train name is required.");

        string inputTrainName = args[0];

        if (inputTrainName == "all")
        {
            int numberOfTrainsUnlocked = 0;

            foreach (Train train in CityModeManager.Instance.Trains)
            {
                if (train.unlocked)
                    continue;

                train.unlocked = true;
                numberOfTrainsUnlocked++;
            }

            ConsoleManager.Instance.Output(
                $"Unlocked {numberOfTrainsUnlocked} new train(s).",
                ConsoleOutputLevel.Success
            );
        }
        else
        {
            Train foundTrain =
                CityModeManager.Instance.Trains.FirstOrDefault(t =>
                    t.trainSO.name.ToLower().Replace(" ", "_") == inputTrainName.ToLower()
                ) ?? throw new Exception($"Could not find train with name \"{inputTrainName}\".");

            if (foundTrain.unlocked)
            {
                ConsoleManager.Instance.Output("Train has already been unlocked.");
                return;
            }

            foundTrain.unlocked = true;
            ConsoleManager.Instance.Output(
                $"{foundTrain.trainSO.name} has been unlocked.",
                ConsoleOutputLevel.Success
            );
        }

        UiManager.Instance.CityModeScreen.trainList.Refresh();
    }
}
