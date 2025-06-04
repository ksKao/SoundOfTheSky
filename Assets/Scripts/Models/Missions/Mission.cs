using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Mission
{
    public const float DEFAULT_SECONDS_PER_MILE = 5f;
    public static readonly Texture2D[] pendingMissionBarBackground =
    {
        UiUtils.LoadTexture("pending_mission_bar_1"),
        UiUtils.LoadTexture("pending_mission_bar_2"),
        UiUtils.LoadTexture("pending_mission_bar_3"),
        UiUtils.LoadTexture("pending_mission_bar_4"),
        UiUtils.LoadTexture("pending_mission_bar_5"),
        UiUtils.LoadTexture("pending_mission_bar_6"),
    };

    private static readonly Texture2D _completeButtonBackground = UiUtils.LoadTexture(
        "complete_button"
    );
    private bool _eventPending = false;
    private Train _train = null;

    public WeatherSO weatherSO;
    protected readonly Label _trainNameLabel = new("Train");
    protected VisualElement weatherUiInPendingMission = new();
    protected int initialMiles = 0;
    protected int milesRemaining = 0;
    protected CheckHealthPanel checkHealthPanel;
    protected VisualElement rewardsContainer = new()
    {
        style =
        {
            width = UiUtils.GetLengthPercentage(100),
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            flexGrow = 1,
            alignItems = Align.FlexStart,
        },
    };

    public abstract MissionType Type { get; }
    public virtual int MilesPerInterval => 5;
    public virtual Passenger[] CrewsAndPassengers => new Passenger[0];
    public virtual Route Route { get; set; } = new();
    public bool IsTutorial { get; protected set; } = false;
    public bool SkippedLastInterval { get; protected set; } = false;
    public MissionStatus MissionStatus { get; set; } = MissionStatus.Pending;
    public float SecondsRemainingUntilNextMile { get; protected set; } = DEFAULT_SECONDS_PER_MILE;
    public Crew[] Crews =>
        CityModeManager.Instance.crews.Where(c => c.deployedMission == this).ToArray();
    public Train Train
    {
        get => _train;
        set
        {
            _train = value;
            if (value is not null)
            {
                if (DeployedMissionUi is not null)
                    DeployedMissionUi.trainImage.sprite = value.trainSO.sprite;
                _trainNameLabel.text = value.trainSO.name;
            }
            else
            {
                if (DeployedMissionUi is not null)
                    DeployedMissionUi.trainImage.sprite = null;
                _trainNameLabel.text = "Train";
            }
        }
    }
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
            if (value && (value != oldValue))
                EventOccur();

            UiManager.Instance.CityModeScreen.bottomNavigationBar.RefreshEventPendingMissionCount();
        }
    }
    public VisualElement PendingMissionUi { get; } = new();
    public DeployedMissionUi DeployedMissionUi { get; protected set; }
    public VisualElement MissionCompleteUi { get; } =
        new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                position = Position.Relative,
            },
        };
    public int MilesRemaining
    {
        get => milesRemaining;
        protected set
        {
            milesRemaining = value;

            OnMileChange();
        }
    }
    public bool IsCompleted { get; protected set; } = false;
    protected virtual Location[] EligibleDestinations =>
        CityModeManager.Instance.Locations.Where((l, i) => i != 0).ToArray();

    static Mission()
    {
        CityModeManager.Instance.OnSelectedPendingMissionChange += static (
            oldMission,
            newMission
        ) =>
        {
            float transitionDuration = .3f,
                marginOffset = 3f;

            if (oldMission is not null)
            {
                oldMission
                    .PendingMissionUi.Query<Button>()
                    .ForEach(button => button.visible = false);
                DOTween.To(
                    () => -marginOffset,
                    x =>
                        oldMission.PendingMissionUi.style.marginLeft = UiUtils.GetLengthPercentage(
                            x
                        ),
                    0,
                    transitionDuration
                );
                DOTween.To(
                    () => marginOffset,
                    x =>
                        oldMission.PendingMissionUi.style.marginRight = UiUtils.GetLengthPercentage(
                            x
                        ),
                    0,
                    transitionDuration
                );
            }

            if (newMission is not null)
            {
                newMission
                    .PendingMissionUi.Query<Button>()
                    .ForEach(button => button.visible = true);
                DOTween.To(
                    () => 0f,
                    x =>
                        newMission.PendingMissionUi.style.marginLeft = UiUtils.GetLengthPercentage(
                            x
                        ),
                    -marginOffset,
                    transitionDuration
                );
                DOTween.To(
                    () => 0f,
                    x =>
                        newMission.PendingMissionUi.style.marginRight = UiUtils.GetLengthPercentage(
                            x
                        ),
                    marginOffset,
                    transitionDuration
                );
            }

            if (
                UiManager.Instance.CityModeScreen.RightPanel
                    == UiManager.Instance.CityModeScreen.crewSelectionPanel
                || UiManager.Instance.CityModeScreen.RightPanel
                    == UiManager.Instance.CityModeScreen.trainList
            )
                UiManager.Instance.CityModeScreen.ChangeRightPanel(null);
        };
    }

    public Mission()
    {
        if (weatherSO == null)
            weatherSO = DataManager.Instance.GetRandomWeather();

        SetupUi();
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
        VisualElement baseContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                width = UiUtils.GetLengthPercentage(100),
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.SpaceBetween,
            },
        };

        baseContainer.Add(new Label("REWARD") { style = { fontSize = 20 } });

        Button completeButton = new()
        {
            text = "COMPLETE",
            style =
            {
                position = Position.Absolute,
                backgroundImage = _completeButtonBackground,
                backgroundColor = Color.clear,
                fontSize = 20,
                color = Color.white,
                top = 0,
                right = 0,
            },
        };
        UiUtils.ToggleBorder(completeButton, false);
        completeButton.clicked += () =>
        {
            CityModeManager.Instance.deployedMissions.Remove(this);
            UiManager.Instance.CityModeScreen.deployedMissionList.Refresh();
        };
        MissionCompleteUi.Add(baseContainer);

        MissionCompleteUi.Add(rewardsContainer);
        MissionCompleteUi.Add(completeButton);
    }

    public virtual void Complete()
    {
        IsCompleted = true;
        DeployedMissionUi.Arrive();
        MissionStatus = MissionStatus.Arrived;

        // when a mission has been completed, there is a 25% chance for resting crews' status to go up by 1
        IEnumerable<Crew> restingCrews = CityModeManager.Instance.crews.Where(c => c.isResting);

        foreach (Crew restingCrew in restingCrews)
        {
            if (Random.ShouldOccur(0.25))
                restingCrew.MakeBetter();

            if (restingCrew.Status == PassengerStatus.Comfortable)
                restingCrew.isResting = false;
        }
    }

    public virtual void Update()
    {
        if (IsCompleted || EventPending)
            return;

        SecondsRemainingUntilNextMile -= Time.deltaTime;

        if (SecondsRemainingUntilNextMile <= 0)
        {
            // reset the timer
            SecondsRemainingUntilNextMile = CityModeManager.Instance.SecondsPerMile;

            MilesRemaining--;
        }
    }

    public virtual void OnResolveButtonClicked()
    {
        EventPending = false;
    }

    public virtual void OnCheckHealthButtonClicked()
    {
        UiManager.Instance.CityModeScreen.ChangeRightPanel(checkHealthPanel);
    }

    public void ApplyCommonPendingMissionUiStyleSingle(VisualElement element, int index)
    {
        element.style.display = DisplayStyle.Flex;
        element.style.flexDirection = FlexDirection.Column;
        element.style.justifyContent =
            element.childCount > 1 ? Justify.SpaceBetween : Justify.Center;
        element.style.alignItems = Align.Center;
        element.style.paddingTop = UiUtils.GetLengthPercentage(1.5f);
        element.style.paddingBottom = UiUtils.GetLengthPercentage(1.5f);
        element.style.paddingLeft = UiUtils.GetLengthPercentage(1.5f);
        element.style.paddingRight = UiUtils.GetLengthPercentage(1.5f);
        element.style.backgroundImage = UiUtils.LoadTexture(
            $"pending_mission_ui_element_background_{index % 5 + 1}"
        );
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

        if (MissionStatus != MissionStatus.Deployed)
            return;

        if (MilesRemaining == 0)
            Complete();
        else if (IsMilestoneReached(MilesPerInterval))
        {
            if (
                Train is not null
                && Random.ShouldOccur(
                    weatherSO.decisionMakingProbability - Train.WarmthLevelPercentage * 0.01
                )
            )
            {
                SkippedLastInterval = false;
                EventPending = true;
            }
            else if (
                Train is not null
                && Random.ShouldOccur(Train.SpeedLevelPercentage * 0.01)
                && !SkippedLastInterval
            ) // when interval is skipped, there is a chance to skip second interval
            {
                SkippedLastInterval = true;
                MilesRemaining = Math.Max(MilesRemaining - MilesPerInterval, 0);
            }
            else if (SkippedLastInterval)
            {
                SkippedLastInterval = false;
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
        PendingMissionUi.Clear();

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
        routeElement.Add(
            UiUtils.WrapLabel(
                new Label(Route.start.locationSO.name + "\n" + Route.end.locationSO.name)
            )
        );
        routeElement.Add(UiUtils.WrapLabel(new Label($"{initialMiles} Miles")));

        PendingMissionUi.Add(routeElement);

        weatherUiInPendingMission = new();
        weatherUiInPendingMission.Add(UiUtils.WrapLabel(new Label(weatherSO.name)));
        weatherUiInPendingMission.Add(
            UiUtils.WrapLabel(new Label(weatherSO.decisionMakingProbability * 100 + "%"))
        );

        PendingMissionUi.Add(weatherUiInPendingMission);
    }

    protected virtual void ApplyCommonPendingMissionUiStyle()
    {
        for (int i = 0; i < PendingMissionUi.childCount; i++)
            ApplyCommonPendingMissionUiStyleSingle(PendingMissionUi.Children().ElementAt(i), i);
    }

    protected virtual void ShowTrainList()
    {
        UiManager.Instance.CityModeScreen.trainList.Show(
            CityModeManager
                .Instance.Trains.Where(t =>
                {
                    int startIndex = Array.FindIndex(
                        CityModeManager.Instance.Locations,
                        (location) => location.locationSO == t.trainSO.routeStartLocation
                    );
                    int endIndex = Array.FindIndex(
                        CityModeManager.Instance.Locations,
                        (location) => location.locationSO == t.trainSO.routeEndLocation
                    );

                    return t.unlocked
                        && startIndex <= Route.startIndex
                        && endIndex >= Route.endIndex
                        && CityModeManager.Instance.deployedMissions.Find(mission =>
                            mission.Train == t
                        )
                            is null; // hide already deployed train
                })
                .ToArray(),
            (train) =>
            {
                Train = train;
                DeployedMissionUi.trainImage.sprite = train.trainSO.sprite;
                UiManager.Instance.CityModeScreen.trainList.activeTrain = train;
                UiManager.Instance.CityModeScreen.trainList.Refresh();
            },
            Train
        );
    }

    protected void AddRewardLabel(string labelText, string rewardIconFileName)
    {
        VisualElement container = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                marginRight = 16,
            },
        };

        container.Add(new Label(labelText));
        container.Add(
            new Image()
            {
                sprite = UiUtils.LoadSprite(rewardIconFileName),
                style = { width = 40, height = 40 },
            }
        );

        rewardsContainer.Add(container);
    }

    protected void SetupUi()
    {
        checkHealthPanel = new(this);

        initialMiles = Route.distance;
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

    private void OnSelectMissionPendingUi(ClickEvent evt)
    {
        CityModeManager.Instance.SelectedPendingMission = this;
    }
}
