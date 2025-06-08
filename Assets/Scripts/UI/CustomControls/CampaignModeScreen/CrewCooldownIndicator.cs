using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewCooldownIndicator : VisualElement
{
    public Label NumberLabel { get; } =
        new()
        {
            text = "0",
            style =
            {
                fontSize = 64,
                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 10,
                marginLeft = 0,
                marginRight = 0,
                marginTop = 0,
                marginBottom = 0,
                height = 64,
                unityTextAlign = TextAnchor.MiddleCenter,
            },
        };

    public CrewCooldownIndicator()
    {
        style.position = Position.Absolute;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
        style.backgroundColor = CampaignModeCrewContainer.backgroundColor;
        style.width = 80;
        style.height = 80;
        style.borderTopLeftRadius = 8;
        style.borderTopRightRadius = 8;
        style.borderBottomLeftRadius = 8;
        style.borderBottomRightRadius = 8;
        style.color = Color.white;
        style.top = UiUtils.GetLengthPercentage(50);
        style.right = 0;
        style.translate = new Translate(
            UiUtils.GetLengthPercentage(50),
            UiUtils.GetLengthPercentage(-50)
        );

        UiUtils.ToggleBorder(this, true, Color.white);
        UiUtils.SetBorderWidth(this, 1);

        Add(NumberLabel);
    }
}
