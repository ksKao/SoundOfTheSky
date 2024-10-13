using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RescueMission : Mission
{
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _crewNumberInput = new("Crew");
    private int _numberOfSupplies = 0;
    private int _numberOfCrews = 0;

    public override MissionType Type { get; } = MissionType.Rescue;
    protected override (LocationSO start, LocationSO end) Route => (Train.routeStartLocation, Train.routeEndLocation);

    public override bool Deploy()
    {
        // check if this train has already been deployed
        if (GameManager.Instance.deployedMissions.Any(m => m.Train != null && m.Train.name == Train.name))
        {
            Debug.Log("Train has already been deployed.");
            return false;
        }

        _numberOfSupplies = _supplyNumberInput.Value;
        _numberOfCrews = _crewNumberInput.Value;

        return true;
    }

    public override VisualElement GenerateDeployedMissionUi()
    {
        VisualElement root = new();
        root.Add(new Label(Route.start.name + " " + Route.end.name));
        return root;
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
    }
}
