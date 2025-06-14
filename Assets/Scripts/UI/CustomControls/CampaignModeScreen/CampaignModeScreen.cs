using DG.Tweening;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeScreen : VisualElement
{
    public readonly WeatherBar weatherBar = new();
    public readonly MainChoicesContainer mainChoicesContainer = new();
    public readonly CampaignModeCrewContainer campaignModeCrewContainer = new();
    public readonly PassengersWindow passengersWindow = new();

    private readonly VisualElement _bottomContainer = new()
    {
        style =
        {
            width = UiUtils.GetLengthPercentage(100),
            flexGrow = 1,
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
        },
    };
    private readonly CampaignModeMenuButton _campaignModeMenuButton = new();

    public CampaignModeScreen()
    {
        style.backgroundImage = UiUtils.LoadTexture("background", Scene.CampaignMode);
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

        Add(topContainer);
        Add(_bottomContainer);
        Add(_campaignModeMenuButton);

        topContainer.Add(passengersWindow);
        topContainer.Add(weatherBar);

        _bottomContainer.Add(
            new Image()
            {
                sprite = UiUtils.LoadSprite("heidi_portrait", Scene.CampaignMode),
                style =
                {
                    height = UiUtils.GetLengthPercentage(95),
                    alignSelf = Align.FlexEnd,
                    width = UiUtils.GetLengthPercentage(25),
                },
            }
        );
        _bottomContainer.Add(mainChoicesContainer);
        _bottomContainer.Add(campaignModeCrewContainer);
    }

    public void ShowBottomContainer()
    {
        DOTween
            .To(
                () => _bottomContainer.style.marginTop.value.value,
                x => _bottomContainer.style.marginTop = UiUtils.GetLengthPercentage(x),
                0,
                0.5f
            )
            .SetEase(Ease.InBounce);
    }

    public void HideBottomContainer(bool transition = true)
    {
        if (transition)
        {
            DOTween
                .To(
                    () => _bottomContainer.style.marginTop.value.value,
                    x => _bottomContainer.style.marginTop = UiUtils.GetLengthPercentage(x),
                    100,
                    0.5f
                )
                .SetEase(Ease.OutBounce);
        }
        else
        {
            _bottomContainer.style.marginTop = UiUtils.GetLengthPercentage(100);
        }
    }
}
