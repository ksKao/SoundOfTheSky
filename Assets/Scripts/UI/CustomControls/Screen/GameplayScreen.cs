using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public VisualElement PendingMissionList { get; private set; } = new();
    public MissionTypeTab MissionTypeTab { get; private set; } = new();

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

        PendingMissionList.style.height = UiUtils.GetLengthPercentage(100);

        left.Add(new AssetsBar());
        left.Add(MissionTypeTab);
        left.Add(PendingMissionList);

        Add(left);
        Add(right);
    }
}
