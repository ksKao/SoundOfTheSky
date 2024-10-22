using UnityEngine.UIElements;

public class DocumentationMission : Mission
{
    private readonly LocationSO _destination = DataManager.Instance.GetRandomDestinationLocation();
    private readonly NumberInput _resourceNumberInput = new("Resource");
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _paymentNumberInput = new("Payment");
    private int _numberOfResources = 0;
    private int _numberOfSupplies = 0;
    private int _numberOfPayments = 0;

    public override MissionType Type { get; } = MissionType.Documentation;
    public override TrainSO Train { get; } = null;
    public override Route Route => new(DataManager.Instance.AllLocations[0], _destination);

    public override bool Deploy()
    {
        _numberOfResources = _resourceNumberInput.Value;
        _numberOfSupplies = _supplyNumberInput.Value;
        _numberOfPayments = _paymentNumberInput.Value;

        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();
    }

    protected override void EventOccur()
    {
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        PendingMissionUi.Add(_resourceNumberInput);
        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_paymentNumberInput);
    }
}
