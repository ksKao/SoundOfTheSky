using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class WeatherBarIcons : VisualElement
{
    public WeatherBarIcons()
    {
        style.position = Position.Absolute;
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.left = UiUtils.GetLengthPercentage(
            100 / (float)CampaignModeManager.NUMBER_OF_FUTURE_WEATHER
        );
    }

    public void Transition()
    {
        DOTween
            .To(
                () => 100 / (float)CampaignModeManager.NUMBER_OF_FUTURE_WEATHER,
                x => style.left = UiUtils.GetLengthPercentage(x),
                0f,
                CampaignModeManager.Instance.DayTransitionDuration
            )
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                style.left = UiUtils.GetLengthPercentage(
                    100 / (float)CampaignModeManager.NUMBER_OF_FUTURE_WEATHER
                );
                RepopulateIcons();
            });
    }

    public void RepopulateIcons()
    {
        Clear();

        foreach (
            (CampaignModeWeatherSO weatherSO, bool hidden) in CampaignModeManager
                .Instance
                .FutureWeathers
        )
        {
            VisualElement container = new()
            {
                style =
                {
                    width = UiUtils.GetLengthPercentage(
                        100 / (float)CampaignModeManager.NUMBER_OF_FUTURE_WEATHER
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
                    sprite = hidden
                        ? UiUtils.LoadSprite("unknown", Scene.CampaignMode)
                        : weatherSO.sprite,
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
