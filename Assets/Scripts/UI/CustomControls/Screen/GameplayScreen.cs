using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public readonly MissionTypeTab missionTypeTab = new();
    public readonly BottomNavigationBar bottomNavigationBar = new();
    public readonly VisualElement pendingMissionList = new();
    public readonly DeployedMissionList deployedMissionList = new();

    private readonly VisualElement _right = new();

    public GameplayScreen()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.minHeight = UiUtils.GetLengthPercentage(100);

        VisualElement left = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(50),
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column
            }
        };
        pendingMissionList.style.height = UiUtils.GetLengthPercentage(100);

        left.Add(new AssetsBar());
        left.Add(missionTypeTab);
        left.Add(pendingMissionList);
        left.Add(bottomNavigationBar);

        Add(left);
        Add(_right);
    }

    public void ChangeRightPanel(VisualElement element)
    {
        _right.Clear();
        _right.Add(element);
    }
}
