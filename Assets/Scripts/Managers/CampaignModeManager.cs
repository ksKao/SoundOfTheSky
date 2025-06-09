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
    private int _temperature = 0;
    private bool _skippedToday = false;

    public int Day
    {
        get => _day;
        set
        {
            _day = value;
            UiManager.Instance.CampaignModeScreen.weatherBar.dayLabel.text = $"DAY {value}";
        }
    }
    public int Temperature
    {
        get => _temperature;
        private set
        {
            _temperature = value;
            UiManager.Instance.CampaignModeScreen.weatherBar.temperatureLabel.text =
                $"{value}{WeatherBar.DEGREE_SYMBOL}";
        }
    }
    public CampaignModeWeatherSO TodaysWeather => FutureWeathers.First();
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
        if (action != null)
        {
            // check if have enough crews
            int numberOfCrewsAvailable = CrewCooldowns.Where(c => c == 0).Count();

            if (numberOfCrewsAvailable < action.crewsNeeded)
            {
                UiUtils.ShowError("You do not have enough crews to perform this action");
                return;
            }

            if (action.type == ActionType.Medical)
            {
                for (int i = 0; i < Passengers.Length; i++)
                {
                    for (int j = 0; j < action.valueIncrease; j++)
                        ChangePassengerHealth(i, true);
                }
            }
            else
            {
                Temperature += action.valueIncrease;
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
        }

        StartCoroutine(TransitionDay());
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

    private IEnumerator TransitionDay()
    {
        // apply health updates based on temperature
        for (int i = 0; i < Passengers.Length; i++)
        {
            (float sickChance, float recoverChance) = Temperature switch
            {
                >= 0 => (0, 0.2f),
                < 0 and > -10 => (0.1f, 0.15f),
                <= -10 and > -20 => (0.2f, 0.1f),
                <= -20 and > -30 => (0.3f, 0.05f),
                <= -30 and > -40 => (0.5f, 0.02f),
                _ => (0.75f, 0),
            };

            if (
                Random.ShouldOccur(sickChance)
                && (!_skippedToday || Passengers[i].status < (PassengerStatus.Death - 1))
            ) // passengers cannot die when today is skipped
                ChangePassengerHealth(i, false);
            if (Random.ShouldOccur(recoverChance) && Passengers[i].status != PassengerStatus.Death) // cannot revive dead passengers
                ChangePassengerHealth(i, true);
        }

        RerollWeather();
        UiManager.Instance.CampaignModeScreen.HideBottomContainer();

        yield return new WaitForSeconds(DAY_TRANSITION_DURATION);

        StartNewDay();
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

    /// <summary>
    /// Changes a passenger health based on index
    /// </summary>
    /// <param name="index">index of the Passengers array</param>
    /// <param name="isMakeBetter">true to make passenger better, false to make passenger worse</param>
    private void ChangePassengerHealth(int index, bool isMakeBetter)
    {
        IEnumerable<int> values = Enum.GetValues(typeof(PassengerStatus)).Cast<int>();
        int min = values.Min();
        int max = values.Max();
        int newStatusInt = (int)Passengers[index].status;

        if (isMakeBetter)
            newStatusInt--;
        else
            newStatusInt++;

        Passengers[index].status = (PassengerStatus)Math.Clamp(newStatusInt, min, max);

        UiManager.Instance.CampaignModeScreen.passengersWindow.Refresh();
    }

    private void StartNewDay()
    {
        Day++;

        if (Random.ShouldOccur(TodaysWeather.chanceOfWarming))
            Temperature += TodaysWeather.temperatureIncrease;
        else
            Temperature -= TodaysWeather.temperatureDecrease;

        for (int i = 0; i < CrewCooldowns.Length; i++)
        {
            CrewCooldowns[i] = Math.Max(CrewCooldowns[i] - 1, 0);
        }

        UiManager.Instance.CampaignModeScreen.campaignModeCrewContainer.RefreshCooldown();

        if (!Random.ShouldOccur(TodaysWeather.eventChance))
        {
            _skippedToday = true;
            ApplyAction(null);
        }
        else
        {
            _skippedToday = false;
            UiManager.Instance.CampaignModeScreen.ShowBottomContainer();
        }
    }
}
