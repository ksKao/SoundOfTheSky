using System.Linq;
using DG.Tweening;
using UnityEngine.UIElements;

[UxmlElement]
public partial class WeatherBarIcons : VisualElement
{
    private string[] _icons = new string[CampaignModeManager.NUMBER_OF_FUTURE_WEATHER_SHOWED + 1]
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    public string[] Icons
    {
        get => _icons;
        set
        {
            string[] prevIcons = _icons;

            _icons = value;

            if (!prevIcons.All(i => string.IsNullOrEmpty(i)))
                Transition();
        }
    }

    public WeatherBarIcons()
    {
        style.position = Position.Absolute;
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.left = UiUtils.GetLengthPercentage(
            100 / (CampaignModeManager.NUMBER_OF_FUTURE_WEATHER_SHOWED + 1)
        );
    }

    public void Transition()
    {
        DOTween
            .To(
                () => 100 / (float)(CampaignModeManager.NUMBER_OF_FUTURE_WEATHER_SHOWED + 1),
                x => style.left = UiUtils.GetLengthPercentage(x),
                0f,
                CampaignModeManager.DAY_TRANSITION_DURATION
            )
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                style.left = UiUtils.GetLengthPercentage(
                    100 / (float)(CampaignModeManager.NUMBER_OF_FUTURE_WEATHER_SHOWED + 1)
                );
                RepopulateIcons();
            });
    }

    private void RepopulateIcons()
    {
        Clear();

        foreach (string icon in _icons)
        {
            VisualElement container = new()
            {
                style =
                {
                    width = UiUtils.GetLengthPercentage(
                        100 / (float)(CampaignModeManager.NUMBER_OF_FUTURE_WEATHER_SHOWED + 1)
                    ),
                    height = UiUtils.GetLengthPercentage(100),
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Column,
                    justifyContent = Justify.Center,
                    alignItems = Align.Center,
                },
            };

            container.Add(
                new Image()
                {
                    sprite = UiUtils.LoadSprite(icon, GameplayMode.CampaignMode),
                    style =
                    {
                        width = UiUtils.GetLengthPercentage(50),
                        height = UiUtils.GetLengthPercentage(50),
                    },
                }
            );

            Add(container);
        }
    }
}
