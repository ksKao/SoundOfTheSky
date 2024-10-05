using UnityEngine.UIElements;

public class ResupplyMission : Mission
{
    private TrainSO _train;
    private int _numberOfSupplies;
    private int _numberOfCrews;
    private int _numberOfResources;

    public override MissionType Type => MissionType.Resupply;

    public override void FillMissionUi(VisualElement parent)
    {
        parent.Add(new Label(Type.ToString()));
    }
}
