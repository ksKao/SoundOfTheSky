using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Mission
{
    private const int SECONDS_PER_MILE = 5;
    private const int MILES_PER_INTERVAL = 5;
    private float secondsRemainingUntilNextMile = SECONDS_PER_MILE;
    private bool _isCompleted = false;

    protected readonly WeatherSO _weather;
    protected readonly int _initialMiles = 0;
    protected int _milesRemaining = 0;

    public abstract MissionType Type { get; }
    public VisualElement PendingMissionUi { get; } = new();
    protected abstract (LocationSO start, LocationSO end) Route { get; }

    protected Mission()
    {
        _weather = DataManager.Instance.GetRandomWeather();

        CalculateMilesRemaining();
        _initialMiles = _milesRemaining;

        GeneratePendingMissionUi();

        // after finish generating UI, make sure the elements are evenly spaced
        foreach (VisualElement child in PendingMissionUi.Children())
        {
            child.style.flexGrow = 1;
        }

        PendingMissionUi.RegisterCallback<ClickEvent>(OnSelectMissionPendingUi);
    }

    ~Mission()
    {
        PendingMissionUi.UnregisterCallback<ClickEvent>(OnSelectMissionPendingUi);
    }

    public abstract void OnDeploy();
    protected abstract void EventOccur();

    public void Update()
    {
        if (_isCompleted) return;

        secondsRemainingUntilNextMile -= Time.deltaTime;

        if (secondsRemainingUntilNextMile <= 0)
        {
            // reset the timer
            secondsRemainingUntilNextMile = SECONDS_PER_MILE;

            _milesRemaining--;

            if (_milesRemaining == 0)
                _isCompleted = true;
            else if ((_initialMiles - _milesRemaining) % MILES_PER_INTERVAL == 0 && new System.Random().NextDouble() <= _weather.decisionMakingProbability)
                EventOccur();
        }
    }
    
    public void OnDeselectMissionPendingUi()
    {
        PendingMissionUi.Query<Button>().ForEach(button => button.visible = false);

        PendingMissionUi.style.borderTopColor = Color.clear;
        PendingMissionUi.style.borderBottomColor = Color.clear;
        PendingMissionUi.style.borderLeftColor = Color.clear;
        PendingMissionUi.style.borderRightColor = Color.clear;

        PendingMissionUi.style.borderTopWidth = 0;
        PendingMissionUi.style.borderBottomWidth = 0;
        PendingMissionUi.style.borderLeftWidth = 0;
        PendingMissionUi.style.borderRightWidth = 0;
    }

    protected virtual void GeneratePendingMissionUi()
    {
        PendingMissionUi.style.height = UiUtils.GetLengthPercentage(100 / GameManager.NUMBER_OF_PENDING_MISSIONS_PER_TYPE);
        PendingMissionUi.style.display = DisplayStyle.Flex;
        PendingMissionUi.style.flexDirection = FlexDirection.Row;

        VisualElement routeElement = new();
        routeElement.Add(new Label(Route.start.name));
        routeElement.Add(new Label(Route.end.name));

        PendingMissionUi.Add(routeElement);

        VisualElement weatherElement = new();
        weatherElement.Add(new Label(_weather.name));
        weatherElement.Add(new Label(_weather.decisionMakingProbability * 100 + "%"));

        PendingMissionUi.Add(weatherElement);

        PendingMissionUi.Add(new Label(Type.ToString()));
    }

    private void CalculateMilesRemaining()
    {
        int startIndex = Array.IndexOf(DataManager.Instance.AllLocations, Route.start);

        for (int i = startIndex; DataManager.Instance.AllLocations[i] != Route.end; i++)
            _milesRemaining += DataManager.Instance.AllLocations[i].milesToNextStop;
    }

    private void OnSelectMissionPendingUi(ClickEvent evt)
    {
        PendingMissionUi.Query<Button>().ForEach(button => button.visible = true);

        PendingMissionUi.style.borderTopColor = Color.black;
        PendingMissionUi.style.borderBottomColor = Color.black;
        PendingMissionUi.style.borderLeftColor = Color.black;
        PendingMissionUi.style.borderRightColor = Color.black;

        PendingMissionUi.style.borderTopWidth = 2;
        PendingMissionUi.style.borderBottomWidth = 2;
        PendingMissionUi.style.borderLeftWidth = 2;
        PendingMissionUi.style.borderRightWidth = 2;

        GameManager.Instance.SelectedPendingMission = this;
    }
}

public enum MissionType
{
    Rescue,
    Resupply,
    Documentation
}
