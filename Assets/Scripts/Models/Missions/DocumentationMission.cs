using UnityEngine.UIElements;

public class DocumentationMission : Mission
{
    private readonly LocationSO _destination = DataManager.Instance.GetRandomDestinationLocation();
    private readonly NumberInput _resourceNumberInput = new("Resource");
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _paymentNumberInput = new("Payment");

    public override MissionType Type { get; } = MissionType.Documentation;
    public override TrainSO Train { get; } = null;
    public override Route Route => new(DataManager.Instance.AllLocations[0], _destination);
    public int NumberOfResources { get; private set; } = 0;
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfPayments { get; private set; } = 0;

    public override bool Deploy()
    {
        NumberOfResources = _resourceNumberInput.Value;
        NumberOfSupplies = _supplyNumberInput.Value;
        NumberOfPayments = _paymentNumberInput.Value;

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

    protected override void EventOccur() { }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        PendingMissionUi.style.position = Position.Relative;

        VisualElement weatherLabelContainer = new();
        weatherLabelContainer.Add(new Label("Weather"));

        PendingMissionUi.Add(weatherLabelContainer);
        PendingMissionUi.Add(_resourceNumberInput);
        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_paymentNumberInput);

        PendingMissionUi.Add(new DocumentationMissionPendingWeatherTree(weatherLabelContainer));
    }
}
