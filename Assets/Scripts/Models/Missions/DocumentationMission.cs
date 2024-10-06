public class DocumentationMission : Mission
{
    private readonly LocationSO _destinationLocation = DataManager.Instance.GetRandomDestinationLocation();
    private readonly NumberInput _resourceNumberInput = new("Resource");
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _paymentNumberInput = new("Payment");
    private int _numberOfResources = 0;
    private int _numberOfSupplies = 0;
    private int _numberOfPayments = 0;

    public override MissionType Type => MissionType.Documentation;

    protected override (LocationSO, LocationSO) Route => (DataManager.Instance.AllLocations[0], _destinationLocation);

    public override void OnDeploy()
    {
        _numberOfResources = _resourceNumberInput.Value;
        _numberOfSupplies = _supplyNumberInput.Value;
        _numberOfPayments = _paymentNumberInput.Value;
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        PendingMissionUi.Add(_resourceNumberInput);
        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_paymentNumberInput);
    }
}
