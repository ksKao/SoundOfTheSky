using UnityEngine.UIElements;

[UxmlElement]
public partial class GameplayScreen : VisualElement
{
    public VisualElement PendingMissionList { get; private set; } = new();
    public MissionTypeTab MissionTypeTab { get; private set; } = new();

    public GameplayScreen()
    {
        style.minHeight = new Length()
        {
            unit = LengthUnit.Percent,
            value = 100
        };

        VisualElement left = new()
        {
            style =
            {
                width = new Length()
                {
                    unit = LengthUnit.Percent,
                    value = 50
                },
                height = new Length()
                {
                    unit = LengthUnit.Percent,
                    value = 100
                },
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column
            }
        };
        VisualElement right = new();

        PendingMissionList.style.height = new Length()
        {
            unit = LengthUnit.Percent,
            value = 100
        };

        left.Add(new AssetsBar());
        left.Add(MissionTypeTab);
        left.Add(PendingMissionList);

        Add(left);
        Add(right);
    }
}
