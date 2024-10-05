public class RescueMission : Mission
{
    private readonly TrainSO _train = DataManager.Instance.GetRandomTrain();
    private int _numberOfSupplies = 0;
    private int _numberOfCrews = 0;

    public override MissionType Type => MissionType.Rescue;

    protected override (LocationSO, LocationSO) Route => (_train.routeStartLocation, _train.routeEndLocation);

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();
    }
}
