public class ResupplyMission : Mission
{
    private readonly TrainSO _train = DataManager.Instance.GetRandomTrain();
    private int _numberOfSupplies = 0;
    private int _numberOfCrews = 0;
    private int _numberOfResources = 0;

    public override MissionType Type => MissionType.Resupply;

    protected override (LocationSO, LocationSO) Route => (_train.routeStartLocation, _train.routeEndLocation);

    public override void OnDeploy()
    {
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();
    }
}
