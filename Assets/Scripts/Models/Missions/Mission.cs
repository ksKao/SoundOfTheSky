using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Mission
{
    // consts
    private const float SECONDS_PER_MILE = 0.1f;

    public static readonly Texture2D[] pendingMissionBarBackground = {
        UiUtils.LoadTexture("pending_mission_bar_1"),
        UiUtils.LoadTexture("pending_mission_bar_2"),
        UiUtils.LoadTexture("pending_mission_bar_3"),
        UiUtils.LoadTexture("pending_mission_bar_4"),
        UiUtils.LoadTexture("pending_mission_bar_5"),
        UiUtils.LoadTexture("pending_mission_bar_6"),
    };

    // state
    private float _secondsRemainingUntilNextMile = SECONDS_PER_MILE;
    private bool _isCompleted = false;
    private bool _eventPending = false;
    private bool _skippedLastInterval = false;

    protected WeatherSO weather;
    protected VisualElement weatherUiInPendingMission = new();
    protected readonly int initialMiles = 0;
    protected int milesRemaining = 0;

    public abstract MissionType Type { get; }
    public abstract Route Route { get; }
    public virtual Train Train { get; } =
        Random.GetFromArray(GameManager.Instance.Trains.ToArray());
    public virtual int MilesPerInterval => 5;
    public Crew[] Crews =>
        GameManager.Instance.crews.Where(c => c.DeployedMission == this).ToArray();
    public WeatherSO WeatherSO => weather;
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
    public VisualElement MissionCompleteUi { get; } = new();
    public int MilesRemaining
    {
        get => milesRemaining;
        protected set
        {
            milesRemaining = value;

            OnMileChange();
        }
    }

    public Mission()
    {
        weather = DataManager.Instance.GetRandomWeather();

        // each tier of the weather will increase the chance by 5%
        int currentWeatherIndex = Array.IndexOf(DataManager.Instance.AllWeathers, weather);

        initialMiles = CalculateInitialMiles();
        MilesRemaining = initialMiles;

        GeneratePendingMissionUi();

        ApplyCommonPendingMissionUiStyle();

        GenerateDeployedMissionUi();

        // after finish generating UI, make sure the elements are evenly spaced
        foreach (VisualElement child in PendingMissionUi.Children())
        {
            child.style.flexGrow = 1;
        }

        PendingMissionUi.RegisterCallback<ClickEvent>(OnSelectMissionPendingUi);
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

    public virtual void GenerateMissionCompleteUi()
    {
        MissionCompleteUi.Add(new Label("Reward"));

        Button completeButton = new() { text = "Complete" };
        completeButton.clicked += () =>
        {
            GameManager.Instance.deployedMissions.Remove(this);
            UiManager.Instance.GameplayScreen.deployedMissionList.Refresh();
        };
        MissionCompleteUi.Add(completeButton);
    }

    public virtual void Complete()
    {
        if (_isCompleted)
            return;

        _isCompleted = true;
        GenerateMissionCompleteUi();
        DeployedMissionUi.Arrive();

        // when a mission has been completed, there is a 25% chance for resting crews' status to go up by 1
        IEnumerable<Crew> restingCrews = GameManager.Instance.crews.Where(c => c.isResting);

        foreach (Crew restingCrew in restingCrews)
        {
            if (Random.ShouldOccur(0.25))
                restingCrew.MakeBetter();

            if (restingCrew.Status == PassengerStatus.Comfortable)
                restingCrew.isResting = false;
        }

        UiManager.Instance.GameplayScreen.crewList.RefreshCrewList();
    }

    public void Update()
    {
        if (_isCompleted || EventPending)
            return;

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

        UiUtils.ToggleBorder(PendingMissionUi, false);

        if (
            UiManager.Instance.GameplayScreen.RightPanel
            == UiManager.Instance.GameplayScreen.crewSelectionPanel
        )
            UiManager.Instance.GameplayScreen.ChangeRightPanel(null);
    }

    public virtual void OnResolveButtonClicked()
    {
        EventPending = false;
    }

    public void ApplyCommonPendingMissionUiStyleSingle(VisualElement element, int index)
    {
        element.style.display = DisplayStyle.Flex;
        element.style.flexDirection = FlexDirection.Column;
        element.style.justifyContent = element.childCount > 1 ? Justify.SpaceBetween : Justify.Center;
        element.style.alignItems = Align.Center;
        element.style.paddingTop = UiUtils.GetLengthPercentage(1.5f);
        element.style.paddingBottom = UiUtils.GetLengthPercentage(1.5f);
        element.style.paddingLeft = UiUtils.GetLengthPercentage(1.5f);
        element.style.paddingRight = UiUtils.GetLengthPercentage(1.5f);
        element.style.backgroundImage = UiUtils.LoadTexture($"pending_mission_ui_element_background_{index % 5 + 1}");
        element.style.maxWidth = UiUtils.GetLengthPercentage(13);
        element.style.height = UiUtils.GetLengthPercentage(100);

        if (element.childCount >= 2)
        {
            element.Children().ElementAt(element.childCount - 1).style.fontSize = 20;
        }
    }

    protected virtual void OnMileChange()
    {
        if (DeployedMissionUi is not null)
            DeployedMissionUi.milesRemainingLabel.text = milesRemaining + " miles";

        if (MilesRemaining == 0)
            Complete();
        else if (IsMilestoneReached(MilesPerInterval))
        {
            if (
                Train is not null
                && Random.ShouldOccur(
                    weather.decisionMakingProbability - Train.WarmthLevelPercentage * 0.01
                )
            )
            {
                _skippedLastInterval = false;
                EventPending = true;
            }
            else if (
                Train is not null
                && Random.ShouldOccur(Train.SpeedLevelPercentage * 0.01)
                && !_skippedLastInterval
            ) // when interval is skipped, there is a chance to skip second interval
            {
                _skippedLastInterval = true;
                MilesRemaining = Math.Max(MilesRemaining - MilesPerInterval, 0);
            }
            else if (_skippedLastInterval)
            {
                _skippedLastInterval = false;
            }
        }
    }

    /// <summary>
    /// Determines if a milestone has been reached based on the specified interval. E.g. if interval is 5, then this will be true for 5, 10, 15, and so on
    /// </summary>
    /// <param name="interval">The interval to check against.</param>
    /// <returns>True if the difference between initial miles and miles remaining is a multiple of the interval; otherwise, false.</returns>
    protected bool IsMilestoneReached(int interval)
    {
        if (initialMiles == MilesRemaining)
            return false;

        return (initialMiles - MilesRemaining) % interval == 0;
    }

    protected virtual void GeneratePendingMissionUi()
    {
        // Each pending mission UI is 20% height because need to fit 5 of them into the list
        PendingMissionUi.style.height = UiUtils.GetLengthPercentage(20);
        PendingMissionUi.style.display = DisplayStyle.Flex;
        PendingMissionUi.style.flexDirection = FlexDirection.Row;
        PendingMissionUi.style.justifyContent = Justify.SpaceEvenly;
        PendingMissionUi.style.alignItems = Align.Center;
        PendingMissionUi.style.paddingTop = UiUtils.GetLengthPercentage(4);
        PendingMissionUi.style.paddingBottom = UiUtils.GetLengthPercentage(4.5f);
        PendingMissionUi.style.paddingLeft = UiUtils.GetLengthPercentage(3);
        PendingMissionUi.style.paddingRight = UiUtils.GetLengthPercentage(3);

        VisualElement routeElement = new();
        routeElement.Add(UiUtils.WrapLabel(new Label(Route.start.locationSO.name + "\n" + Route.end.locationSO.name)));
        routeElement.Add(UiUtils.WrapLabel(new Label(initialMiles.ToString())));

        PendingMissionUi.Add(routeElement);

        weatherUiInPendingMission.Add(UiUtils.WrapLabel(new Label(weather.name)));
        weatherUiInPendingMission.Add(UiUtils.WrapLabel(new Label(weather.decisionMakingProbability * 100 + "%")));

        PendingMissionUi.Add(weatherUiInPendingMission);
    }

    protected virtual void ApplyCommonPendingMissionUiStyle()
    {
        for (int i = 0; i < PendingMissionUi.childCount; i++)
            ApplyCommonPendingMissionUiStyleSingle(PendingMissionUi.Children().ElementAt(i), i);
    }

    private int CalculateInitialMiles()
    {
        int initialMiles = 0;
        int startIndex = Array.IndexOf(GameManager.Instance.Locations, Route.start);

        for (int i = startIndex; GameManager.Instance.Locations[i] != Route.end; i++)
            initialMiles += DataManager.Instance.AllLocations[i].milesToNextStop;

        return initialMiles;
    }

    private void OnSelectMissionPendingUi(ClickEvent evt)
    {
        PendingMissionUi.Query<Button>().ForEach(button => button.visible = true);

        GameManager.Instance.SelectedPendingMission = this;
    }
}
