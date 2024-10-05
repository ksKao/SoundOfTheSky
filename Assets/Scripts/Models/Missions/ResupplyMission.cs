using UnityEngine.UIElements;

public class ResupplyMission : Mission
{
    private TrainSO _train;
    private int _numberOfSupplies;
    private int _numberOfCrews;
    private int _numberOfResources;

    public override MissionType Type => MissionType.Resupply;

    public override VisualElement GenerateMissionUI()
    {
        VisualElement root = new();

        root.Add(new Label(Type.ToString()));

        return root;
    }
}
