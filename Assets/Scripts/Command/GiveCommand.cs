using System;
using System.Collections.Generic;
using System.Linq;

public class GiveCommand : Command
{
    public override string Id => "give";

    public override string HelpText => "Give materials";

    public override Dictionary<string, string> Usage
    {
        get
        {
            Dictionary<string, string> usage = new();

            foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
            {
                string key = $"give {materialType.ToString().ToLower()} <amount>";

                if (materialType != MaterialType.Citizens && materialType != MaterialType.Residents)
                {
                    usage.Add(
                        key,
                        $"Give x amount of {materialType}. Negative numbers are allowed."
                    );
                }
                else
                {
                    usage.Add(
                        key + " <location>",
                        $"Increase number of {materialType.ToString().ToLower()} in x location (case insensitive). If location contains space, replace it with underscore."
                    );
                }
            }

            return usage;
        }
    }

    public override void Execute(string[] args)
    {
        if (args.Length < 2)
            throw new Exception("Missing argument.");

        string materialStr = args[0];
        string amountStr = args[1];

        if (!Enum.TryParse(materialStr, ignoreCase: true, out MaterialType material))
            throw new Exception($"\"{materialStr}\" is not a valid material.");

        if (!int.TryParse(amountStr, out int amount))
            throw new Exception($"\"{amountStr}\" is not a valid integer.");

        if (material == MaterialType.Citizens || material == MaterialType.Residents)
        {
            if (args.Length < 3)
                throw new Exception(
                    $"Location name is required if material is {material.ToString().ToLower()}."
                );

            Location foundLocation =
                GameManager.Instance.Locations.FirstOrDefault(l =>
                    l.locationSO.name.ToLower().Replace(" ", "_") == args[2]
                ) ?? throw new Exception("Invalid location name.");

            int amountBefore = foundLocation.Residents;
            int amountAfter = amountBefore + amount;

            if (material == MaterialType.Citizens)
                foundLocation.Citizens += amount;
            else
                foundLocation.Residents += amount;

            ConsoleManager.Instance.Output(
                $"{foundLocation.locationSO.name} {material.ToString().ToLower()}: {amountBefore} -> {amountAfter}",
                ConsoleOutputLevel.Success
            );
        }
        else
        {
            int amountBefore = GameManager.Instance.GetMaterialValue(material);
            int amountAfter = amountBefore + amount;

            GameManager.Instance.IncrementMaterialValue(material, amount);
            ConsoleManager.Instance.Output(
                $"{material}: {amountBefore} -> {amountAfter}",
                ConsoleOutputLevel.Success
            );
        }
    }
}
