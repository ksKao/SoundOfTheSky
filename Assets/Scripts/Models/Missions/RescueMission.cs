using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RescueMission : Mission
{
    private const int MILES_PER_PASSENGER_INCREASE = 5;
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _crewNumberInput = new("Crew");
    private readonly double _passengerIncreaseProbability = 0.5f; // determines the probability that the train will have 1 more passenger (50% base chance)
    private readonly RescueMissionResolvePanel _rescueMissionResolvePanel = null;
    private int _numberOfSupplies = 0;
    private int _numberOfCrews = 0;

    public override Route Route => new(Train.routeStartLocation, Train.routeEndLocation);
    public override MissionType Type { get; } = MissionType.Rescue;
    public List<Passenger> Passengers { get; } = new();

    public RescueMission() : base()
    {
        int weatherIndex = Array.IndexOf(DataManager.Instance.AllWeathers, weather);
        _passengerIncreaseProbability += weatherIndex * 0.05; // each weather difficulty will additionally increase the probability to get a passenger by 5%
        _rescueMissionResolvePanel = new(this);
    }

    public override bool Deploy()
    {
        // check if this train has already been deployed
        if (GameManager.Instance.deployedMissions.Any(m => m.Train != null && m.Train.name == Train.name))
            return false;

        _numberOfSupplies = _supplyNumberInput.Value;
        _numberOfCrews = _crewNumberInput.Value;

        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();
    }

    public override void OnResolveButtonClicked()
    {
        // cannot call base here since need to wait until player make a decision before continuing
        _rescueMissionResolvePanel.RefreshUi();
        UiManager.Instance.GameplayScreen.ChangeRightPanel(_rescueMissionResolvePanel);
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();

        if (IsMilestoneReached(MILES_PER_PASSENGER_INCREASE) && Random.ShouldOccur(_passengerIncreaseProbability))
        {
            Debug.Log(initialMiles + " " + MilesRemaining + " " + ((initialMiles - MilesRemaining) % 5));
            Passengers.Add(new());
        }
    }

    protected override void EventOccur()
    {
        // if there are no passengers, can set event pending back to false
        // this can happen in the very early stage of the mission, where the 50% chance of getting a new passenger does not occur
        // which can cause confusion as there are no passengers but there is still an event
        if (Passengers.Count == 0)
        {
            EventPending = false;
            return;
        }

        foreach (Passenger passenger in Passengers)
        {
            // 50% chance for a passenger health to change
            if (Random.ShouldOccur(0.5))
            {
                // 50% to increase health, 50% to decrease health
                if (Random.ShouldOccur(0.5))
                    passenger.IncrementStatus();
                else
                    passenger.DecrementStatus();
            }
        }

        // remove all dead passengers
        Passengers.RemoveAll(p => p.Status == PassengerStatus.Death);
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        PendingMissionUi.Add(new Label(Train.name));

        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_crewNumberInput);
    }
}
