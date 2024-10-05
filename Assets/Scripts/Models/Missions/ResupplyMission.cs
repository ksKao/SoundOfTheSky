public class ResupplyMission : Mission
{
    private TrainSO _train = DataManager.Instance.GetRandomTrain();
    private int _numberOfSupplies;
    private int _numberOfCrews;
    private int _numberOfResources;

    public override MissionType Type => MissionType.Resupply;

    protected override (LocationSO, LocationSO) Route => (_train.routeStartLocation, _train.routeEndLocation);

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();
    }
}
