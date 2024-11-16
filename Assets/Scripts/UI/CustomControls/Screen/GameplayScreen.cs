using System.Linq;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public readonly AssetsBar assetBar = new();
    public readonly MissionTypeTab missionTypeTab = new();
    public readonly BottomNavigationBar bottomNavigationBar = new();
    public readonly VisualElement pendingMissionList = new();
    public readonly DeployedMissionList deployedMissionList = new();
    public readonly CrewSelectionPanel crewSelectionPanel = new();

    private readonly VisualElement _right = new();

    public VisualElement RightPanel => _right.Children().FirstOrDefault();

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

        left.Add(assetBar);
        left.Add(missionTypeTab);
        left.Add(pendingMissionList);
        left.Add(bottomNavigationBar);

        _right.style.width = UiUtils.GetLengthPercentage(50);

        Add(left);
        Add(_right);
    }

    public void ChangeRightPanel(VisualElement element)
    {
        if (RightPanel == element) return;

        _right.Clear();

        if (element is not null) _right.Add(element);
    }
}
