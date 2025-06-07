using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeScreen : VisualElement
{
    public readonly WeatherBar weatherBar = new();
    public readonly MainChoicesContainer mainChoicesContainer = new();
    public readonly CampaignModeCrewContainer campaignModeCrewContainer = new();

    public CampaignModeScreen()
    {
        style.backgroundImage = UiUtils.LoadTexture("background", GameplayMode.CampaignMode);
        style.minHeight = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.unityFont = Resources.Load<Font>("Fonts/myriad_pro");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/myriad_pro")
        );

        VisualElement topContainer = new()
        {
            style =
            {
                position = Position.Relative,
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(30),
            },
        };

        VisualElement bottomContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(100),
                flexGrow = 1,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
            },
        };

        Add(topContainer);
        Add(bottomContainer);

        topContainer.Add(new PassengersWindow());
        topContainer.Add(weatherBar);

        bottomContainer.Add(
            new Image()
            {
                sprite = UiUtils.LoadSprite("heidi_portrait", GameplayMode.CampaignMode),
                style = { height = UiUtils.GetLengthPercentage(95), alignSelf = Align.FlexEnd },
            }
        );
        bottomContainer.Add(mainChoicesContainer);
        bottomContainer.Add(campaignModeCrewContainer);
    }
}
