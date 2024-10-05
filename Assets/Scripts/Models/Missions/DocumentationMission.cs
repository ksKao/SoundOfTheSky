using UnityEngine.UIElements;

public class DocumentationMission : Mission
{
    private int _numberOfResources;
    private int _numberOfSupplies;
    private int _numberOfPayments;

    public override MissionType Type => MissionType.Documentation;

    public override void FillMissionUi(VisualElement parent)
    {
        parent.Add(new Label(MissionType.Documentation.ToString()));
    }
}
