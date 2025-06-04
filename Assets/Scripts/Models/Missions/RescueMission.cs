using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class RescueMission : Mission
{
    // UIs
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly double _passengerIncreaseProbability = 0.5f; // determines the probability that the train will have 1 more passenger (50% base chance)
    private readonly RescueMissionResolvePanel _rescueMissionResolvePanel = null;
    private readonly Label _deployedMissionCrewLabel = new();
    private readonly Label _deployedMissionPassengerLabel = new();
    private readonly CrewSelectionPanelButton _crewSelectionPanelButton = new();
    private readonly List<Crew> _crewsOnCooldown = new();

    // state
    private bool _actionTakenDuringThisEvent = false;

    public List<Crew> CrewsOnCooldown => _crewsOnCooldown;
    public override Route Route { get; set; } = new(false);
    public override MissionType Type { get; } = MissionType.Rescue;
    public List<Passenger> Passengers { get; } = new();
    public override Passenger[] CrewsAndPassengers
    {
        get
        {
            Passenger[] crewsAndPassengers = new Passenger[Passengers.Count + Crews.Length];

            Passengers.CopyTo(crewsAndPassengers, 0);
            Crews.CopyTo(crewsAndPassengers, Passengers.Count);

            return crewsAndPassengers;
        }
    }
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfResources { get; private set; } = 0;
    public int NumberOfDeaths { get; private set; } = 0;
    public int NumberOfResidents { get; private set; } = 0;
    public int NumberOfNewResources { get; private set; } = 0;
    public bool ActionTakenDuringThisEvent
    {
        get => _actionTakenDuringThisEvent;
        set
        {
            _actionTakenDuringThisEvent = value;

            // cannot ignore event if already used supply/crew on passenger
            if (
                _rescueMissionResolvePanel is not null
                && _rescueMissionResolvePanel.ignoreButton is not null
            )
                _rescueMissionResolvePanel.ignoreButton.visible = !value;
        }
    }

    public RescueMission()
        : base()
    {
        int weatherIndex = Array.IndexOf(DataManager.Instance.AllWeathers, weatherSO);
        _passengerIncreaseProbability += weatherIndex * 0.05; // each weather difficulty will additionally increase the probability to get a passenger by 5%
        _rescueMissionResolvePanel = new(this);
    }

    public RescueMission(PendingMissionSerializable pendingMissionSerializable)
        : this()
    {
        Route = new Route(
            pendingMissionSerializable.routeStart,
            pendingMissionSerializable.routeEnd
        );

        WeatherSO foundWeather = DataManager.Instance.AllWeathers.FirstOrDefault(w =>
            w.name == pendingMissionSerializable.weather
        );

        if (foundWeather)
            weatherSO = foundWeather;

        SetupUi();
    }

    public RescueMission(bool isTutorial)
        : this()
    {
        IsTutorial = isTutorial;

        if (!IsTutorial)
            return;

        Route = new Route("Whittier", "Spencer"); // only 10 miles

        weatherSO = DataManager.Instance.AllWeathers.FirstOrDefault();

        SetupUi();
    }

    public RescueMission(DeployedRescueMissionSerializable deployedRescueMissionSerializable)
        : this()
    {
        Route = new Route(
            deployedRescueMissionSerializable.routeStart,
            deployedRescueMissionSerializable.routeEnd
        );

        WeatherSO foundWeather = DataManager.Instance.AllWeathers.FirstOrDefault(w =>
            w.name == deployedRescueMissionSerializable.weather
        );

        if (foundWeather)
            weatherSO = foundWeather;

        Train = CityModeManager.Instance.Trains.FirstOrDefault(t =>
            t.trainSO.name == deployedRescueMissionSerializable.trainName
        );

        Passengers = deployedRescueMissionSerializable
            .passengers.Select(p => new Passenger(p))
            .ToList();

        SecondsRemainingUntilNextMile =
            deployedRescueMissionSerializable.secondsRemainingUntilNextMile;

        foreach (string crewId in deployedRescueMissionSerializable.crewIds)
        {
            Crew found = CityModeManager.Instance.crews.Find(c => c.id == crewId);

            if (found is null)
                continue;

            found.deployedMission = this;
        }

        CrewsOnCooldown.Clear();
        foreach (string crewId in deployedRescueMissionSerializable.crewIdsOnCooldown)
        {
            Crew found = CityModeManager.Instance.crews.Find(c => c.id == crewId);

            if (found is null)
                continue;

            CrewsOnCooldown.Add(found);
        }

        IsCompleted = deployedRescueMissionSerializable.isCompleted;

        NumberOfSupplies = deployedRescueMissionSerializable.numberOfSupplies;

        NumberOfResources = deployedRescueMissionSerializable.numberOfResources;

        NumberOfDeaths = deployedRescueMissionSerializable.numberOfDeaths;

        NumberOfResidents = deployedRescueMissionSerializable.numberOfResidents;

        NumberOfNewResources = deployedRescueMissionSerializable.numberOfNewResources;

        SkippedLastInterval = deployedRescueMissionSerializable.skippedLastInterval;

        SetupUi();

        DeployedMissionUi.trainImage.sprite = Train.trainSO.sprite;

        MilesRemaining = deployedRescueMissionSerializable.milesRemaining;

        DeployedMissionUi.StyleIndex = deployedRescueMissionSerializable.deployedMissionStyleIndex;

        MissionStatus = deployedRescueMissionSerializable.status;

        ActionTakenDuringThisEvent = deployedRescueMissionSerializable.actionTakenDuringThisEvent;

        _deployedMissionPassengerLabel.text = $"{Passengers.Count} passenger(s)";
        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";

        EventPending = deployedRescueMissionSerializable.eventPending;

        if (MissionStatus == MissionStatus.Arrived)
            DeployedMissionUi.Arrive();
        else if (MissionStatus == MissionStatus.Completed)
        {
            GenerateMissionCompleteUi();
            DeployedMissionUi.Complete();
        }
    }

    public override bool Deploy()
    {
        if (Train is null)
        {
            UiUtils.ShowError("Please select a train before proceeding");
            return false;
        }

        if (!Train.unlocked)
        {
            UiUtils.ShowError("You must unlock this train first before deploying");
            return false;
        }

        // check if this train has already been deployed
        if (
            CityModeManager.Instance.deployedMissions.Any(m =>
                m.Train != null && m.Train.trainSO.name == Train.trainSO.name
            )
        )
        {
            UiUtils.ShowError("Train has already been deployed");
            return false;
        }

        if (_crewSelectionPanelButton.SelectedCrews.Any(c => c.deployedMission is not null))
        {
            UiUtils.ShowError("One or more crew(s) has already been deployed in another mission");
            return false;
        }

        if (
            CityModeManager.Instance.GetMaterialValue(MaterialType.Supplies)
            < _supplyNumberInput.Value
        )
        {
            UiUtils.ShowError("Not enough supplies to deploy this mission");
            return false;
        }

        NumberOfSupplies = _supplyNumberInput.Value;
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Supplies, -NumberOfSupplies);

        foreach (Crew crew in _crewSelectionPanelButton.SelectedCrews)
            crew.deployedMission = this;

        _deployedMissionPassengerLabel.text = $"{Passengers.Count} passenger(s)";
        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";

        MissionStatus = MissionStatus.Deployed;
        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();

        DeployedMissionUi.materialLabelsContainer.Add(_deployedMissionCrewLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_deployedMissionPassengerLabel);
    }

    public void UseSupply(Passenger[] selectedPassengers)
    {
        if (selectedPassengers.Length == 0)
        {
            UiUtils.ShowError("No passengers selected");
            return;
        }

        Passenger[] uncomfortablePassengers = selectedPassengers
            .Where(p => p.Status != PassengerStatus.Comfortable)
            .ToArray();

        if (uncomfortablePassengers.Length == 0)
        {
            UiUtils.ShowError("All passengers are already comfortable");
            return;
        }

        if (NumberOfSupplies < uncomfortablePassengers.Length)
        {
            UiUtils.ShowError("Not enough supplies.");
            return;
        }

        // can use selected passengers instead of uncomfortable passengers here since calling make better doesnt make a different, and we want to deselect all anyways
        foreach (Passenger passenger in selectedPassengers)
        {
            passenger.MakeBetter();
            passenger.Selected = false;
        }

        NumberOfSupplies -= uncomfortablePassengers.Length;
        _rescueMissionResolvePanel.RefreshButtonText();

        ActionTakenDuringThisEvent = true;
    }

    public void UseCrew(Crew selectedCrew)
    {
        Passenger[] selectedPassengers = Passengers.Where(p => p.Selected).ToArray();
        Crew[] selectedCrews = Crews.Where(c => c.Selected).ToArray();

        // this means that the player probably wants to select multiple crews/passengers to use supply
        if (selectedPassengers.Length > 1 || selectedCrews.Length > 1)
            return;

        // heal passengers with crew
        if (selectedPassengers.Length == 1)
        {
            Passenger selectedPassenger = selectedPassengers.FirstOrDefault();

            if (selectedPassenger is null)
            {
                UiUtils.ShowError("No passengers selected");
                selectedCrew.Selected = false;
                return;
            }

            if (selectedPassenger.Status == PassengerStatus.Comfortable)
            {
                UiUtils.ShowError("Passenger is already comfortable.");
                selectedPassenger.Selected = false;
                selectedCrew.Selected = false;
                return;
            }

            if (_crewsOnCooldown.Contains(selectedCrew))
            {
                UiUtils.ShowError("This crew is currently on cooldown");
                selectedCrew.Selected = false;
                return;
            }

            // the chance of passenger's health decreasing is same as the weather event occur probability
            if (!Random.ShouldOccur(weatherSO.decisionMakingProbability))
            {
                selectedPassenger.MakeBetter();

                // chance for passenger status to go up by 2
                if (Random.ShouldOccur(selectedCrew.MedicLevelPercentage * 0.01))
                    selectedPassenger.MakeBetter();
            }

            selectedPassenger.Selected = false;
            selectedCrew.Selected = false;

            _rescueMissionResolvePanel.RefreshButtonText();
            ActionTakenDuringThisEvent = true;

            _crewsOnCooldown.Add(selectedCrew);
            selectedCrew.bracketLabel.text = "(Cooldown)";
            selectedCrew.bracketLabel.style.display = DisplayStyle.Flex;
        }
    }

    public void Ignore()
    {
        // technically should not be possible to reach this condition, but add here just in case if this method is called by something other than the on click on Ignore button
        if (ActionTakenDuringThisEvent)
        {
            UiUtils.ShowError("Cannot ignore event after using supply or crew.");
            return;
        }

        EventPending = false;

        foreach (Passenger passenger in Passengers)
            passenger.Selected = false;

        ActionTakenDuringThisEvent = false;
        UiManager.Instance.CityModeScreen.ChangeRightPanel(
            UiManager.Instance.CityModeScreen.deployedMissionList
        );
    }

    public void Finish()
    {
        // if didn't use supply or crew, then this event remains unresolved.
        if (!ActionTakenDuringThisEvent)
        {
            UiUtils.ShowError("Cannot finish resolve event without first using supply or crew.");
            return;
        }

        ActionTakenDuringThisEvent = false;
        UiManager.Instance.CityModeScreen.ChangeRightPanel(
            UiManager.Instance.CityModeScreen.deployedMissionList
        );

        EventPending = false;
    }

    public override void OnResolveButtonClicked()
    {
        // cannot call base here since need to wait until player make a decision before continuing
        UiManager.Instance.CityModeScreen.ChangeRightPanel(_rescueMissionResolvePanel);
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();

        AddRewardLabel($"{NumberOfResidents} New Residents!", "reward_residents");
        AddRewardLabel($"{NumberOfNewResources} Resources!", "reward_resources");
        AddRewardLabel($"{NumberOfDeaths} Deaths!", "reward_deaths");
    }

    public override void Complete()
    {
        // calculate rewards
        double rewardMultiplier = 1 + weatherSO.rewardMultiplier;
        NumberOfResidents = (int)
            Math.Round(
                Passengers.Where(p => p.Status != PassengerStatus.Death).ToArray().Length
                    * rewardMultiplier
            );
        NumberOfNewResources = (int)Math.Round(NumberOfResources * rewardMultiplier);

        // include city documented citizens bonus
        NumberOfNewResources += Route.end.Citizens * 5;

        CityModeManager.Instance.IncrementMaterialValue(
            MaterialType.Resources,
            NumberOfNewResources
        );
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Supplies, NumberOfSupplies);
        Route.end.Residents += NumberOfResidents;

        foreach (Crew crew in Crews)
            crew.deployedMission = null;

        base.Complete();
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();

        if (IsMilestoneReached(MilesPerInterval))
        {
            // 15% chance to refresh crew usage
            _crewsOnCooldown.RemoveAll(c =>
            {
                if (Random.ShouldOccur(0.15))
                {
                    c.bracketLabel.text = "";
                    c.bracketLabel.style.display = DisplayStyle.None;
                    return true;
                }

                c.bracketLabel.text = "(Cooldown)";
                c.bracketLabel.style.display = DisplayStyle.Flex;
                return false;
            });

            if (Random.ShouldOccur(_passengerIncreaseProbability))
            {
                NumberOfResources += Train.CartLevel;

                if (Random.ShouldOccur(_passengerIncreaseProbability))
                {
                    Passengers.Add(new());
                    checkHealthPanel.Refresh();
                }

                _deployedMissionPassengerLabel.text = $"{Passengers.Count} passenger(s)";
            }
        }
    }

    protected override void EventOccur()
    {
        Passenger[] crewsAndPassengers = CrewsAndPassengers; // cache
        foreach (Passenger passenger in crewsAndPassengers)
        {
            // 50% chance for a passenger health to change
            if (Random.ShouldOccur(0.5))
            {
                // 50% to increase health, 50% to decrease health
                if (Random.ShouldOccur(0.5))
                    passenger.MakeWorse();
                else
                    passenger.MakeBetter();
            }
        }

        // remove all dead passengers and crews
        int numberOfPassengers = Passengers.Count;
        Passengers.RemoveAll(p => p.Status == PassengerStatus.Death);
        NumberOfDeaths += numberOfPassengers - Passengers.Count;

        CityModeManager.Instance.crews.RemoveAll(c => c.Status == PassengerStatus.Death);

        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";
        _deployedMissionPassengerLabel.text = $"{Passengers.Count} passenger(s)";

        // if there are no passengers, can set event pending back to false
        // this can happen in the very early stage of the mission, where the 50% chance of getting a new passenger does not occur
        // which can cause confusion as there are no passengers but there is still an event
        // also check for if all passengers have status of comfortable, otherwise there is no reason for an event to happen
        if (
            crewsAndPassengers.Length == 0
            || crewsAndPassengers.All(p => p.Status == PassengerStatus.Comfortable)
        )
            EventPending = false;
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        VisualElement labelWrapper = new();

        labelWrapper.Add(UiUtils.WrapLabel(_trainNameLabel));
        labelWrapper.RegisterCallback<ClickEvent>((e) => ShowTrainList());

        PendingMissionUi.Add(labelWrapper);

        PendingMissionUi.Add(_supplyNumberInput);

        PendingMissionUi.Add(_crewSelectionPanelButton);
    }
}
