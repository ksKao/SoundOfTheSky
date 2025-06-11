using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignModeManager : Singleton<CampaignModeManager>
{
    public const int MAX_DAYS = 243;
    public const int DAY_TRANSITION_DURATION = 5;
    public const int NUMBER_OF_FUTURE_WEATHER = 6;
    public const int NUMBER_OF_CREWS = 5;
    public const int NUMBER_OF_PASSENGERS = 20;
    public const float WEATHER_HIDDEN_CHANCE = 0.2f;

    private int _day = 1;
    private int _temperature = 0;
    private bool _skippedToday = false;
    private bool _transitioning = false;

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
        set
        {
            _temperature = value;
            UiManager.Instance.CampaignModeScreen.weatherBar.temperatureLabel.text =
                $"{value}{WeatherBar.DEGREE_SYMBOL}";
        }
    }
    public int DayTransitionDuration { get; set; } = DAY_TRANSITION_DURATION;
    public int MaxDays { get; set; } = MAX_DAYS;
    public CampaignModeWeatherSO TodaysWeather => FutureWeathers.First().weather;
    public List<(CampaignModeWeatherSO weather, bool hidden)> FutureWeathers { get; } =
        new(NUMBER_OF_FUTURE_WEATHER);
    public (string name, PassengerStatus status)[] Passengers { get; } =
        new (string, PassengerStatus)[NUMBER_OF_PASSENGERS];
    public int[] CrewCooldowns { get; } = new int[NUMBER_OF_CREWS] { 0, 0, 0, 0, 0 };
    public static string SaveFilePath
    {
        get
        {
            int index = PlayerPrefs.GetInt(SaveMenu.PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY, -1);
            return GetSaveFilePath(index);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Application.runInBackground = true;

        InputManager.Instance.InputAction.CampaignMode.OpenConsole.performed += ctx =>
            ConsoleManager.Instance.OpenConsole();
    }

    private void OnEnable()
    {
        InputManager.Instance.InputAction.CampaignMode.Enable();
    }

    private void Start()
    {
        UiManager.Instance.CampaignModeScreen.mainChoicesContainer.RefreshTab();
        StartGame();

        LoadGame();
    }

    private void OnDisable()
    {
        InputManager.Instance.InputAction.CampaignMode.Disable();
    }

    public static string GetSaveFilePath(int index)
    {
        return Path.Combine(Application.persistentDataPath, $"campaign_mode_{index}.json");
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

    public bool SaveGame()
    {
        CampaignModeState campaignModeState = new()
        {
            day = Day,
            temperature = Temperature,
            skippedToday = _skippedToday,
            transitioning = _transitioning,
            futureWeathers = FutureWeathers
                .Select(w => new CampaignModeWeatherSerializable()
                {
                    name = w.weather.name,
                    hidden = w.hidden,
                })
                .ToArray(),
            statuses = Passengers.Select(p => p.status).ToArray(),
            crewCooldowns = CrewCooldowns,
        };

        try
        {
            string serialized = JsonUtility.ToJson(campaignModeState, true);

            using (FileStream stream = new(SaveFilePath, FileMode.Create))
            {
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(serialized);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error while saving data: " + e);
            return false;
        }
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
            FutureWeathers.Add(
                (
                    Random.GetFromArray(DataManager.Instance.AllCampaignModeWeathers),
                    Random.ShouldOccur(WEATHER_HIDDEN_CHANCE)
                )
            );
        }

        UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.RepopulateIcons();
    }

    private IEnumerator TransitionDay()
    {
        // apply health updates based on temperature
        if (!_transitioning)
        {
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
                if (Random.ShouldOccur(recoverChance)) // cannot revive dead passengers
                    ChangePassengerHealth(i, true);
            }

            RerollWeather();
            UiManager.Instance.CampaignModeScreen.HideBottomContainer();
            _transitioning = true;
        }
        else
        {
            UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.Transition();
        }

        yield return new WaitForSeconds(DayTransitionDuration);
        _transitioning = false;

        StartNewDay();
    }

    private void RerollWeather()
    {
        for (int i = 1; i < NUMBER_OF_FUTURE_WEATHER; i++)
        {
            FutureWeathers[i - 1] = FutureWeathers[i];
        }

        FutureWeathers[^1] = (
            Random.GetFromArray(DataManager.Instance.AllCampaignModeWeathers),
            Random.ShouldOccur(WEATHER_HIDDEN_CHANCE)
        );

        UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.Transition();
    }

    /// <summary>
    /// Changes a passenger health based on index
    /// </summary>
    /// <param name="index">index of the Passengers array</param>
    /// <param name="isMakeBetter">true to make passenger better, false to make passenger worse</param>
    private void ChangePassengerHealth(int index, bool isMakeBetter)
    {
        // cannot revive dead passengers
        if (isMakeBetter && Passengers[index].status == PassengerStatus.Death)
            return;

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

        if (Passengers.All(p => p.status == PassengerStatus.Death))
            Lose();
    }

    private void StartNewDay()
    {
        Day++;

        if (Day >= MaxDays)
            Win();

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

    private void Win()
    {
        SceneManager.LoadScene((int)Scene.MainMenu);
    }

    private void Lose()
    {
        SceneManager.LoadScene((int)Scene.MainMenu);
    }

    private void LoadGame()
    {
        if (!File.Exists(SaveFilePath))
            return;

        CampaignModeState savedData = null;

        try
        {
            string serialized = "";

            using (FileStream stream = new(SaveFilePath, FileMode.Open))
            {
                using (StreamReader reader = new(stream))
                {
                    serialized = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrWhiteSpace(serialized))
                return;

            savedData = JsonUtility.FromJson<CampaignModeState>(serialized);
        }
        catch (Exception e)
        {
            Debug.LogError("Error while reading saved data: " + e);
        }

        if (savedData is null)
            return;

        Day = savedData.day;
        Temperature = savedData.temperature;
        _skippedToday = savedData.skippedToday;
        _transitioning = savedData.transitioning;

        FutureWeathers.Clear();

        foreach (
            CampaignModeWeatherSerializable campaignModeWeatherSerializable in savedData.futureWeathers
        )
        {
            FutureWeathers.Add(
                (
                    DataManager.Instance.AllCampaignModeWeathers.First(w =>
                        w.name == campaignModeWeatherSerializable.name
                    ),
                    campaignModeWeatherSerializable.hidden
                )
            );
        }

        for (int i = 0; i < savedData.statuses.Length; i++)
        {
            Passengers[i].status = savedData.statuses[i];
        }

        for (int i = 0; i < savedData.crewCooldowns.Length; i++)
        {
            CrewCooldowns[i] = savedData.crewCooldowns[i];
        }

        UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.RepopulateIcons();
        UiManager.Instance.CampaignModeScreen.passengersWindow.Refresh();

        if (_transitioning)
        {
            UiManager.Instance.CampaignModeScreen.HideBottomContainer(false);
            ApplyAction(null);
        }
    }
}
