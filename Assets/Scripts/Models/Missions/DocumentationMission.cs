public class DocumentationMission : Mission
{
    private LocationSO _destinationLocation = DataManager.Instance.GetRandomDestinationLocation();
    private int _numberOfResources;
    private int _numberOfSupplies;
    private int _numberOfPayments;

    public override MissionType Type => MissionType.Documentation;

    protected override (LocationSO, LocationSO) Route => (DataManager.Instance.AllLocations[0], _destinationLocation);

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();
    }
}
