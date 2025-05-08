using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    // state
    private bool _actionTakenDuringThisEvent = false;
    private int _numberOfResidents = 0;
    private int _numberOfNewResources = 0;
    private int _numberOfDeaths = 0;

    public override Route Route =>
        new(Train.trainSO.routeStartLocation, Train.trainSO.routeEndLocation);
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
    public bool ActionTakenDuringThisEvent
    {
        get => _actionTakenDuringThisEvent;
        set
        {
            _actionTakenDuringThisEvent = value;

            // cannot ignore event if already used supply/crew on passenger
            _rescueMissionResolvePanel.ignoreButton.visible = !value;
        }
    }

    public RescueMission()
        : base()
    {
        int weatherIndex = Array.IndexOf(DataManager.Instance.AllWeathers, weather);
        _passengerIncreaseProbability += weatherIndex * 0.05; // each weather difficulty will additionally increase the probability to get a passenger by 5%
        _rescueMissionResolvePanel = new(this);
    }

    public override bool Deploy()
    {
        // check if this train has been unlocked
        if (!Train.unlocked)
        {
            UiUtils.ShowError("You must unlock this train first before deploying");
            return false;
        }

        // check if this train has already been deployed
        if (
            GameManager.Instance.deployedMissions.Any(m =>
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

        if (GameManager.Instance.GetMaterialValue(MaterialType.Supplies) < _supplyNumberInput.Value)
        {
            UiUtils.ShowError("Not enough supplies to deploy this mission");
            return false;
        }

        NumberOfSupplies = _supplyNumberInput.Value;
        GameManager.Instance.IncrementMaterialValue(MaterialType.Supplies, -NumberOfSupplies);

        foreach (Crew crew in _crewSelectionPanelButton.SelectedCrews)
            crew.deployedMission = this;

        _deployedMissionPassengerLabel.text = $"{Passengers.Count} passenger(s)";
        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";

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

            // the chance of passenger's health decreasing is same as the weather event occur probability
            if (Random.ShouldOccur(weather.decisionMakingProbability))
                selectedPassenger.MakeWorse();
            else
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
        UiManager.Instance.GameplayScreen.ChangeRightPanel(
            UiManager.Instance.GameplayScreen.deployedMissionList
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
        UiManager.Instance.GameplayScreen.ChangeRightPanel(
            UiManager.Instance.GameplayScreen.deployedMissionList
        );
        EventPending = false;
    }

    public override void OnResolveButtonClicked()
    {
        // cannot call base here since need to wait until player make a decision before continuing
        UiManager.Instance.GameplayScreen.ChangeRightPanel(_rescueMissionResolvePanel);
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();

        AddRewardLabel($"{_numberOfResidents} New Residents!", "reward_residents");
        AddRewardLabel($"{_numberOfNewResources} Resources!", "reward_resources");
        AddRewardLabel($"{_numberOfDeaths} Deaths!", "reward_deaths");
    }

    public override void Complete()
    {
        // calculate rewards
        double rewardMultiplier = 1 + weather.rewardMultiplier;
        _numberOfResidents = (int)
            Math.Round(
                Passengers.Where(p => p.Status != PassengerStatus.Death).ToArray().Length
                    * rewardMultiplier
            );
        _numberOfNewResources = (int)Math.Round(NumberOfResources * rewardMultiplier);

        // include city documented citizens bonus
        _numberOfNewResources += Route.end.Citizens * 5;

        GameManager.Instance.IncrementMaterialValue(MaterialType.Resources, _numberOfNewResources);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Supplies, NumberOfSupplies);
        Route.end.Residents += _numberOfResidents;

        foreach (Crew crew in Crews)
            crew.deployedMission = null;

        base.Complete();
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();

        if (
            IsMilestoneReached(MilesPerInterval)
            && Random.ShouldOccur(_passengerIncreaseProbability)
        )
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

    protected override void EventOccur()
    {
        Passenger[] allPassengers = Passengers.Concat(Crews).ToArray();
        foreach (Passenger passenger in allPassengers)
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
        _numberOfDeaths += numberOfPassengers - Passengers.Count;

        GameManager.Instance.crews.RemoveAll(c => c.Status == PassengerStatus.Death);

        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";
        _deployedMissionPassengerLabel.text = $"{Passengers.Count} passenger(s)";

        // if there are no passengers, can set event pending back to false
        // this can happen in the very early stage of the mission, where the 50% chance of getting a new passenger does not occur
        // which can cause confusion as there are no passengers but there is still an event
        // also check for if all passengers have status of comfortable, otherwise there is no reason for an event to happen
        if (
            allPassengers.Length == 0
            || allPassengers.All(p => p.Status == PassengerStatus.Comfortable)
        )
            EventPending = false;
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        VisualElement labelWrapper = new();

        labelWrapper.Add(UiUtils.WrapLabel(new(Train.trainSO.name)));
        PendingMissionUi.Add(labelWrapper);

        PendingMissionUi.Add(_supplyNumberInput);

        PendingMissionUi.Add(_crewSelectionPanelButton);
    }
}
