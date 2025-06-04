using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class WeatherBar : VisualElement
{
    public const string DEGREE_SYMBOL = "°";
    public readonly Label temperatureLabel = new()
    {
        text = $"0{DEGREE_SYMBOL}",
        style =
        {
            unityTextAlign = TextAnchor.MiddleCenter,
            paddingLeft = 0,
            paddingRight = 0,
            marginLeft = 0,
            marginRight = 0,
            fontSize = 24,
        },
    };
    public readonly Label dayLabel = new()
    {
        text = $"DAY 0",
        style =
        {
            unityTextAlign = TextAnchor.MiddleCenter,
            paddingLeft = 0,
            paddingRight = 0,
            marginLeft = 0,
            marginRight = 0,
        },
    };

    public WeatherBar()
    {
        style.backgroundColor = new Color(0.29f, 0.36f, 0.38f, 0.7f);
        style.width = UiUtils.GetLengthPercentage(80);
        style.height = UiUtils.GetLengthPercentage(18);
        style.position = Position.Absolute;
        style.top = UiUtils.GetLengthPercentage(8);
        style.right = UiUtils.GetLengthPercentage(3);
        style.borderTopLeftRadius = 12;
        style.borderTopRightRadius = 12;
        style.borderBottomLeftRadius = 12;
        style.borderBottomRightRadius = 12;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.paddingTop = 8;
        style.paddingBottom = 8;
        style.paddingLeft = 8;
        style.paddingRight = 8;

        AspectRatio leftContainer = new()
        {
            modifyParentStyle = false,
            WidthRatio = 1,
            HeightRatio = 1,
            style =
            {
                backgroundColor = Color.white,
                color = new Color(0, 0.44f, 0.74f),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceAround,
                borderTopLeftRadius = 4,
                borderTopRightRadius = 4,
                borderBottomLeftRadius = 4,
                borderBottomRightRadius = 4,
                unityFont = Resources.Load<Font>("Fonts/ronix"),
                unityFontDefinition = new StyleFontDefinition(
                    Resources.Load<FontAsset>("Fonts/ronix")
                ),
            },
        };

        leftContainer.Add(temperatureLabel);
        leftContainer.Add(dayLabel);

        Add(leftContainer);

        VisualElement centerContainer = new()
        {
            style =
            {
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                flexGrow = 1,
            },
        };

        Add(centerContainer);

        VisualElement rightContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
            },
        };

        Add(rightContainer);
    }
}
