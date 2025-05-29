using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DocumentationMissionPendingWeatherTree : VisualElement
{
    private readonly List<VisualElement> _weatherContainers = new(
        DataManager.Instance.AllWeathers.Length
    );

    public DocumentationMissionPendingWeatherTree()
    {
        Debug.LogWarning(
            $"Detected calling default constructor of {nameof(DocumentationMissionPendingWeatherTree)}."
        );
    }

    public DocumentationMissionPendingWeatherTree(DocumentationMission mission)
    {
        style.position = Position.Absolute;
        style.left = 0;
        style.top = UiUtils.GetLengthPercentage(150);
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;
        style.justifyContent = Justify.SpaceEvenly;
        style.backgroundImage = UiUtils.LoadTexture("pending_mission_bar_1");
        style.paddingTop = UiUtils.GetLengthPercentage(4);
        style.paddingBottom = UiUtils.GetLengthPercentage(4.5f);
        style.paddingLeft = UiUtils.GetLengthPercentage(3);
        style.paddingRight = UiUtils.GetLengthPercentage(3);

        for (int i = 0; i < mission.WeatherProbabilities.Count; i++)
        {
            KeyValuePair<WeatherSO, int> weatherProbability =
                mission.WeatherProbabilities.ElementAt(i);

            VisualElement container = new();
            container.Add(UiUtils.WrapLabel(new Label(weatherProbability.Key.name)));
            container.Add(
                new Image()
                {
                    sprite = weatherProbability.Key.sprite,
                    style =
                    {
                        width = UiUtils.GetLengthPercentage(45),
                        height = UiUtils.GetLengthPercentage(45),
                    },
                }
            );
            container.Add(UiUtils.WrapLabel(new Label(weatherProbability.Value * 10 + "%")));
            container.style.color = Color.white;

            mission.ApplyCommonPendingMissionUiStyleSingle(container, i);
            container.style.fontSize = 12;
            container.style.backgroundImage = UiUtils.LoadTexture(
                "weather_distribution_background"
            );
            container.style.width = UiUtils.GetLengthPercentage(
                100 / DataManager.Instance.AllWeathers.Length
            );

            Add(container);
            _weatherContainers.Add(container);
        }

        generateVisualContent += (ctx) =>
        {
            Painter2D painter = ctx.painter2D;
            float lineWidth = 10;
            painter.lineWidth = lineWidth;
            painter.strokeColor = UiUtils.HexToRgb("#bde6f6");

            Vector2 firstPosition = new();
            Vector2 lastPosition = new();

            for (int i = 0; i < _weatherContainers.Count; i++)
            {
                VisualElement container = _weatherContainers[i];

                Vector2 lineStartOffset = new(0, 10); // add few pixels offset because of the background image not filling the dimension
                painter.BeginPath();
                Vector2 lineStart =
                    container.layout.position
                    + new Vector2(container.layout.width / 2, 0)
                    + lineStartOffset;
                Vector2 lineEnd = lineStart + new Vector2(0, -container.layout.height);
                painter.MoveTo(lineStart);
                painter.LineTo(lineEnd);
                painter.Stroke();

                if (i == 0)
                    firstPosition = lineEnd;
                else if (i == _weatherContainers.Count - 1)
                    lastPosition = lineEnd;
            }

            Rect rect = mission.WeatherLabelContainer.ChangeCoordinatesTo(this, paddingRect);
            Vector2 weatherLabelContainer6OClock =
                rect.position
                + new Vector2(
                    mission.WeatherLabelContainer.resolvedStyle.width / 2,
                    mission.WeatherLabelContainer.resolvedStyle.height
                );
            painter.BeginPath();
            painter.MoveTo(weatherLabelContainer6OClock);
            painter.LineTo(new Vector2(weatherLabelContainer6OClock.x, firstPosition.y));
            painter.Stroke();

            painter.BeginPath();
            painter.MoveTo(firstPosition + new Vector2(-lineWidth / 2, 0));
            painter.LineTo(lastPosition + new Vector2(lineWidth / 2, 0));
            painter.Stroke();
        };
    }
}
