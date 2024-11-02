using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ResupplyMission : Mission
{
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _crewNumberInput = new("Crew");
    private readonly NumberInput _resourceNumberInput = new("Resource");

    public override MissionType Type { get; } = MissionType.Resupply;
    public override Route Route => new(Train.routeStartLocation, Train.routeEndLocation);
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfCrews { get; private set; } = 0;
    public int NumberOfResources { get; private set; } = 0;


    public override bool Deploy()
    {
        // check if this train has already been deployed
        if (GameManager.Instance.deployedMissions.Any(m => m.Train != null && m.Train.name == Train.name))
        {
            Debug.Log("Train has already been deployed.");
            return false;
        }

        NumberOfSupplies = _supplyNumberInput.Value;
        NumberOfCrews = _crewNumberInput.Value;
        NumberOfResources = _resourceNumberInput.Value;

        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();
    }

    protected override void EventOccur()
    {
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        PendingMissionUi.Add(new Label(Train.name));

        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_crewNumberInput);
        PendingMissionUi.Add(_resourceNumberInput);
    }
}
