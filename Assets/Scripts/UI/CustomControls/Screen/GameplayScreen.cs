using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public readonly VisualElement pendingMissionList = new();
    public readonly MissionTypeTab missionTypeTab = new();
    public readonly BottomNavigationBar bottomNavigationBar = new();

    public GameplayScreen()
    {
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
        VisualElement right = new();

        pendingMissionList.style.height = UiUtils.GetLengthPercentage(100);

        left.Add(new AssetsBar());
        left.Add(missionTypeTab);
        left.Add(pendingMissionList);
        left.Add(bottomNavigationBar);

        Add(left);
        Add(right);
    }
}
