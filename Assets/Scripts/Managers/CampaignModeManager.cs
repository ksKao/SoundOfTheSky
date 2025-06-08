using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampaignModeManager : Singleton<CampaignModeManager>
{
    public const int MAX_DAYS = 243;
    public const int DAY_TRANSITION_DURATION = 5;
    public const int NUMBER_OF_FUTURE_WEATHER = 6;
    public const int NUMBER_OF_CREWS = 5;
    public const int NUMBER_OF_PASSENGERS = 20;

    private int _day = 1;

    public int Day
    {
        get => _day;
        set
        {
            _day = value;
            UiManager.Instance.CampaignModeScreen.weatherBar.dayLabel.text = $"DAY {value}";
        }
    }
    public List<CampaignModeWeatherSO> FutureWeathers { get; } = new(NUMBER_OF_FUTURE_WEATHER);
    public (string name, PassengerStatus status)[] Passengers { get; } =
        new (string, PassengerStatus)[NUMBER_OF_PASSENGERS];
    public int[] CrewCooldowns { get; } = new int[NUMBER_OF_CREWS] { 0, 0, 0, 0, 0 };

    private void Start()
    {
        UiManager.Instance.CampaignModeScreen.mainChoicesContainer.RefreshTab();
        StartGame();
    }

    public void ApplyAction(ActionSO action)
    {
        // check if have enough crews
        int numberOfCrewsAvailable = CrewCooldowns.Where(c => c == 0).Count();

        if (numberOfCrewsAvailable < action.crewsNeeded)
        {
            UiUtils.ShowError("You do not have enough crews to perform this action");
            return;
        }

        int cooldownsApplied = 0;
        for (int i = 0; i < CrewCooldowns.Length && cooldownsApplied < action.crewsNeeded; i++)
        {
            if (CrewCooldowns[i] != 0)
                continue;

            CrewCooldowns[i] = Random.GetRandomIntInRangeInclusive(
                action.cooldownMin,
                action.cooldownMax
            );

            cooldownsApplied++;
        }
        UiManager.Instance.CampaignModeScreen.campaignModeCrewContainer.RefreshCooldown();

        StartCoroutine(TransitionDay());
    }

    private IEnumerator TransitionDay()
    {
        RerollWeather();
        UiManager.Instance.CampaignModeScreen.HideBottomContainer();

        yield return new WaitForSeconds(DAY_TRANSITION_DURATION);

        Day++;

        for (int i = 0; i < CrewCooldowns.Length; i++)
        {
            CrewCooldowns[i] = Math.Max(CrewCooldowns[i] - 1, 0);
        }

        UiManager.Instance.CampaignModeScreen.ShowBottomContainer();
    }

    private void StartGame()
    {
        Day = 1;

        string[] names =
        {
            "Mark Stousey",
            "Tom Homan",
            "Barret Fore",
            "Kyle Ritten",
            "Bobby Cannon",
        };

        // populate passengers
        for (int i = 0; i < Passengers.Length; i++)
        {
            Passengers[i] = (names[i % names.Length], PassengerStatus.Comfortable);
        }

        UiManager.Instance.CampaignModeScreen.passengersWindow.Refresh();

        for (int i = 0; i < NUMBER_OF_FUTURE_WEATHER; i++)
        {
            FutureWeathers.Add(Random.GetFromArray(DataManager.Instance.AllCampaignModeWeathers));
        }

        UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.RepopulateIcons();
    }

    private void RerollWeather()
    {
        for (int i = 1; i < NUMBER_OF_FUTURE_WEATHER; i++)
        {
            FutureWeathers[i - 1] = FutureWeathers[i];
        }

        FutureWeathers[^1] = Random.GetFromArray(DataManager.Instance.AllCampaignModeWeathers);

        UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.Transition();
    }
}
