using UnityEngine.UIElements;

[UxmlElement]
public partial class BottomNavigationBar : VisualElement
{
    public readonly Button missionButton = new() { text = "Mission" };
    public readonly Button crewButton = new() { text = "Crew" };
    public readonly Button trainButton = new() { text = "Train" };
    public readonly Button deployButton = new() { text = "Deploy" };

    public BottomNavigationBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.SpaceBetween;

        VisualElement buttonGroup = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
            }
        };

        deployButton.visible = false;

        buttonGroup.Add(missionButton);
        buttonGroup.Add(crewButton);
        buttonGroup.Add(trainButton);
        buttonGroup.Add(deployButton);
        Add(buttonGroup);

        Button missionListButton = new()
        {
            text = "Train Button"
        };
        Add(missionListButton);
    }
}
