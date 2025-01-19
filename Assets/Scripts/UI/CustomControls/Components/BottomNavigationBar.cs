using UnityEngine.UIElements;

[UxmlElement]
public partial class BottomNavigationBar : VisualElement
{
    public readonly Button crewButton = new() { text = "Crew" };
    public readonly Button trainButton = new() { text = "Train" };
    public readonly Button deployButton = new() { text = "Deploy" };

    public BottomNavigationBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.SpaceBetween;

        VisualElement buttonGroup =
            new() { style = { display = DisplayStyle.Flex, flexDirection = FlexDirection.Row } };

        crewButton.clicked += () =>
        {
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.crewList
            );
        };

        trainButton.clicked += () =>
        {
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.trainList
            );
        };

        deployButton.visible = false;

        buttonGroup.Add(crewButton);
        buttonGroup.Add(trainButton);
        buttonGroup.Add(deployButton);
        Add(buttonGroup);

        Button deployedMissionListButton = new() { text = "Deployed Missions" };
        deployedMissionListButton.clicked += () =>
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.deployedMissionList
            );
        Add(deployedMissionListButton);
    }
}
