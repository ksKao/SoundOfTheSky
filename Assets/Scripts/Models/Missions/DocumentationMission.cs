using UnityEngine.UIElements;

public class DocumentationMission : Mission
{
    private int _numberOfResources;
    private int _numberOfSupplies;
    private int _numberOfPayments;

    public override MissionType Type => MissionType.Documentation;

    public override VisualElement GenerateMissionUI()
    {
        VisualElement root = new();

        root.Add(new Label(MissionType.Documentation.ToString()));

        return root;
    }
}
