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
    public readonly WeatherBarIcons weatherBarIcons = new();

    public WeatherBar()
    {
        style.backgroundColor = new Color(0.29f, 0.36f, 0.38f, 0.7f);
        style.width = UiUtils.GetLengthPercentage(80);
        style.height = UiUtils.GetLengthPercentage(60);
        style.position = Position.Absolute;
        style.top = 60;
        style.right = 30;
        style.borderTopLeftRadius = 12;
        style.borderTopRightRadius = 12;
        style.borderBottomLeftRadius = 12;
        style.borderBottomRightRadius = 12;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.paddingTop = 16;
        style.paddingBottom = 16;
        style.paddingLeft = 16;
        style.paddingRight = 16;

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
                marginRight = 24,
            },
        };

        leftContainer.Add(temperatureLabel);
        leftContainer.Add(dayLabel);

        Add(leftContainer);

        VisualElement centerContainer = new()
        {
            style =
            {
                position = Position.Relative,
                flexGrow = 1,
                overflow = Overflow.Hidden,
            },
        };

        AspectRatio heidiIconContainer = new()
        {
            modifyParentStyle = false,
            WidthRatio = 1,
            HeightRatio = 1,
        };

        heidiIconContainer.Add(
            new Image()
            {
                sprite = UiUtils.LoadSprite("heidi_icon_square", GameplayMode.CampaignMode),
            }
        );

        centerContainer.Add(weatherBarIcons);
        centerContainer.Add(heidiIconContainer);
        Add(centerContainer);

        VisualElement rightContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                maxWidth = UiUtils.GetLengthPercentage(15),
                justifyContent = Justify.SpaceEvenly,
                marginLeft = 24,
            },
        };

        rightContainer.Add(
            new Image() { sprite = UiUtils.LoadSprite("arrivals_badge", GameplayMode.CampaignMode) }
        );

        rightContainer.Add(
            new Label()
            {
                text = $"{CampaignModeManager.MAX_DAYS} Days",
                style =
                {
                    color = new Color(0, 0.44f, 0.74f),
                    unityFontStyleAndWeight = FontStyle.Bold,
                    backgroundColor = Color.white,
                    borderTopLeftRadius = 4,
                    borderTopRightRadius = 4,
                    borderBottomLeftRadius = 4,
                    borderBottomRightRadius = 4,
                    fontSize = 24,
                    paddingTop = 10,
                    paddingBottom = 10,
                    paddingLeft = 16,
                    paddingRight = 16,
                    marginLeft = 0,
                    marginRight = 0,
                },
            }
        );

        Add(rightContainer);
    }
}
