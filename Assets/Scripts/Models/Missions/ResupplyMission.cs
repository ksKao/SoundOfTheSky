using UnityEngine.UIElements;

public class ResupplyMission : Mission
{
    private readonly TrainSO _train = DataManager.Instance.GetRandomTrain();
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _crewNumberInput = new("Crew");
    private readonly NumberInput _resourceNumberInput = new("Resource");
    private int _numberOfSupplies = 0;
    private int _numberOfCrews = 0;
    private int _numberOfResources = 0;

    public override MissionType Type => MissionType.Resupply;

    protected override (LocationSO, LocationSO) Route => (_train.routeStartLocation, _train.routeEndLocation);

    public override void OnDeploy()
    {
        _numberOfSupplies = _supplyNumberInput.Value;
        _numberOfCrews = _crewNumberInput.Value;
        _numberOfResources = _resourceNumberInput.Value;
    }

    protected override void EventOccur()
    {
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        PendingMissionUi.Add(new Label(_train.name));

        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_crewNumberInput);
        PendingMissionUi.Add(_resourceNumberInput);
    }
}
