using UnityEngine.UIElements;

public class RescueMission : Mission
{
    private TrainSO _train;
    private int _numberOfSupplies = 0;
    private int _numberOfCrews = 0;

    public override MissionType Type => MissionType.Rescue;

    public RescueMission() : base()
    {
        _train = DataManager.Instance.GetRandomTrain();
        _routeStartLocation = _train.routeStartLocation;
        _routeEndLocation = _train.routeEndLocation;
    }

    public override void FillMissionUi(VisualElement parent)
    {
        parent.Add(new Label(Type.ToString()));
    }
}
