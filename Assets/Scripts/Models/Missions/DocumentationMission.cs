public class DocumentationMission : Mission
{
    private readonly LocationSO _destinationLocation = DataManager.Instance.GetRandomDestinationLocation();
    private int _numberOfResources = 0;
    private int _numberOfSupplies = 0;
    private int _numberOfPayments = 0;

    public override MissionType Type => MissionType.Documentation;

    protected override (LocationSO, LocationSO) Route => (DataManager.Instance.AllLocations[0], _destinationLocation);

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();
    }
}
