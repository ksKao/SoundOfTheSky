using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Mission
{
    private const int SECONDS_PER_MILE = 1;
    private const int MILES_PER_INTERVAL = 5;
    private int _milesRemaining = 0;
    private float _secondsRemainingUntilNextMile = SECONDS_PER_MILE;
    private bool _isCompleted = false;
    private bool _eventPending = false;

    protected readonly WeatherSO weather;
    protected readonly int initialMiles = 0;

    public abstract MissionType Type { get; }
    public abstract Route Route { get; }
    public virtual TrainSO Train { get; } = DataManager.Instance.GetRandomTrain();
    public int NumberOfPassengers { get; protected set; } = 0;
    public bool EventPending 
    {
        get => _eventPending;
        protected set
        {
            bool oldValue = _eventPending;
            _eventPending = value;

            if (DeployedMissionUi is not null)
                DeployedMissionUi.resolveButton.visible = value;

            // if set event pending to true and the value is different than the previous one, call event occur
            // need to check if value is different than previous one in case accidentally call multiple times
            if (value && value != oldValue) 
                EventOccur();
        }
    }
    public VisualElement PendingMissionUi { get; } = new();
    public DeployedMissionUi DeployedMissionUi { get; protected set; }
    public int MilesRemaining 
    {
        get => _milesRemaining;
        protected set 
        {
            _milesRemaining = value;

            OnMileChange();

            if (DeployedMissionUi is null) return;
            DeployedMissionUi.milesRemainingLabel.text = _milesRemaining.ToString();
        }
    }

    protected Mission()
    {
        weather = DataManager.Instance.GetRandomWeather();

        // each tier of the weather will increase the chance by 5%
        int currentWeatherIndex = Array.IndexOf(DataManager.Instance.AllWeathers, weather);

        initialMiles = CalculateInitialMiles();
        MilesRemaining = initialMiles;

        GeneratePendingMissionUi();
        GenerateDeployedMissionUi();

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

    /// <summary>
    /// Initialize a mission when it is deployed
    /// </summary>
    /// <returns>A boolean which represents whether the deployment is successful</returns>
    public abstract bool Deploy();

    protected abstract void EventOccur();

    public virtual void GenerateDeployedMissionUi()
    {
        DeployedMissionUi = new(this);
    }

    public void Update()
    {
        if (_isCompleted || EventPending) return;

        _secondsRemainingUntilNextMile -= Time.deltaTime;

        if (_secondsRemainingUntilNextMile <= 0)
        {
            // reset the timer
            _secondsRemainingUntilNextMile = SECONDS_PER_MILE;

            MilesRemaining--;
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

    public virtual void OnResolveButtonClicked()
    {
        EventPending = false;
    }

    protected virtual void OnMileChange()
    {
        if (MilesRemaining == 0)
            _isCompleted = true;
        else if (IsMilestoneReached(MILES_PER_INTERVAL) && Random.ShouldOccur(weather.decisionMakingProbability))
            EventPending = true;
    }

    /// <summary>
    /// Determines if a milestone has been reached based on the specified interval.
    /// </summary>
    /// <param name="interval">The interval to check against.</param>
    /// <returns>True if the difference between initial miles and miles remaining is a multiple of the interval; otherwise, false.</returns>
    protected bool IsMilestoneReached(int interval)
    {
        if (initialMiles == MilesRemaining) return false;

        return (initialMiles - MilesRemaining) % interval == 0;
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
        weatherElement.Add(new Label(weather.name));
        weatherElement.Add(new Label(weather.decisionMakingProbability * 100 + "%"));

        PendingMissionUi.Add(weatherElement);

        PendingMissionUi.Add(new Label(Type.ToString()));
    }

    private int CalculateInitialMiles()
    {
        int initialMiles = 0;
        int startIndex = Array.IndexOf(DataManager.Instance.AllLocations, Route.start);

        for (int i = startIndex; DataManager.Instance.AllLocations[i] != Route.end; i++)
            initialMiles += DataManager.Instance.AllLocations[i].milesToNextStop;

        return initialMiles;
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
