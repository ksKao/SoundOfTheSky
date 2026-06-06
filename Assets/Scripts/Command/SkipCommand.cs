using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkipCommand : Command
{
    public override string Id => "skip";

    public override string HelpText =>
        "Skip the current day's story. Only runnable when story mode is active.";

    public override Dictionary<string, string> Usage =>
        new() { { "skip", "Skip the current day's story" } };

    public override void Execute(string[] args)
    {
        // check if we are even in campaign mode first
        UIDocument uiDocument = Object.FindFirstObjectByType<UIDocument>();

        if (uiDocument == null)
        {
            ConsoleManager.Instance.Output(
                "Unknown error occured. UI document not found.",
                ConsoleOutputLevel.Error
            );
            return;
        }

        CampaignModeScreen campaignModeScreen =
            uiDocument.rootVisualElement.Q<CampaignModeScreen>();
        if (campaignModeScreen == null)
        {
            ConsoleManager.Instance.Output(
                "You are not in campaign mode.",
                ConsoleOutputLevel.Error
            );
            return;
        }

        if (!UiManager.Instance.CampaignModeScreen.IsDialog)
        {
            ConsoleManager.Instance.Output("You are not in story mode.", ConsoleOutputLevel.Error);
            return;
        }

        AudioManager.Instance.StopAllAudio();
        UiManager.Instance.CampaignModeScreen.ChangeToGameplay();
        ConsoleManager.Instance.Output("Skipped");
    }
}
