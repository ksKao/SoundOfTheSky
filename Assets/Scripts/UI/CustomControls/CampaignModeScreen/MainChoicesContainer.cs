using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MainChoicesContainer : VisualElement
{
    public readonly Label topText = new()
    {
        style =
        {
            color = Color.white,
            whiteSpace = WhiteSpace.Normal,
            paddingLeft = 16,
            paddingRight = 16,
            paddingTop = 16,
            paddingBottom = 16,
            marginTop = 0,
            marginBottom = 0,
            marginLeft = 0,
            marginRight = 0,
            minHeight = UiUtils.GetLengthPercentage(10),
        },
    };

    public MainChoicesContainer()
    {
        style.backgroundColor = Color.black;
        style.opacity = 0.93f;
        style.height = UiUtils.GetLengthPercentage(95);
        style.width = UiUtils.GetLengthPercentage(45);
        style.alignSelf = Align.Center;
        style.borderTopLeftRadius = 8;
        style.borderTopRightRadius = 8;
        style.borderBottomLeftRadius = 8;
        style.borderBottomRightRadius = 8;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.alignItems = Align.Center;

        UiUtils.ToggleBorder(this, true, Color.white);
        UiUtils.SetBorderWidth(this, 1);

        Add(topText);

        VisualElement tabsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                width = UiUtils.GetLengthPercentage(60),
                justifyContent = Justify.SpaceBetween,
            },
        };

        Button warmingTab = new()
        {
            text = "Warming",
            style =
            {
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                fontSize = 20,
                backgroundColor = UiUtils.HexToRgb("#c1272c"),
                color = Color.white,
                width = UiUtils.GetLengthPercentage(40),
            },
        };

        UiUtils.ToggleBorder(warmingTab, false);

        Button medicalTab = new()
        {
            text = "Medical",
            style =
            {
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                fontSize = 20,
                backgroundColor = Color.white,
                color = UiUtils.HexToRgb("#c1272c"),
                width = UiUtils.GetLengthPercentage(40),
            },
        };

        UiUtils.ToggleBorder(medicalTab, false);

        tabsContainer.Add(warmingTab);
        tabsContainer.Add(medicalTab);

        Add(tabsContainer);
    }
}
