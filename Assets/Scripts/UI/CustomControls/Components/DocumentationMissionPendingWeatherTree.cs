using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DocumentationMissionPendingWeatherTree : VisualElement
{
    private readonly List<VisualElement> _weatherContainers =
        new(DataManager.Instance.AllWeathers.Length);

    public DocumentationMissionPendingWeatherTree()
    {
        Debug.LogWarning(
            $"Detected calling default constructor of {nameof(DocumentationMissionPendingWeatherTree)}."
        );
    }

    public DocumentationMissionPendingWeatherTree(DocumentationMission mission)
    {
        style.position = Position.Absolute;
        style.top = UiUtils.GetLengthPercentage(150);
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;

        foreach (KeyValuePair<WeatherSO, int> weatherProbability in mission.WeatherProbabilities)
        {
            VisualElement container = new();
            container.style.width = UiUtils.GetLengthPercentage(
                100 / DataManager.Instance.AllWeathers.Length
            );
            container.style.height = UiUtils.GetLengthPercentage(100);
            container.Add(new Label(weatherProbability.Key.name));
            container.Add(new Label(weatherProbability.Value * 10 + "%"));
            Add(container);
            _weatherContainers.Add(container);
        }

        generateVisualContent += (ctx) =>
        {
            Painter2D painter = ctx.painter2D;
            painter.lineWidth = 2;
            painter.strokeColor = Color.black;

            Vector2 firstPosition = new();
            Vector2 lastPosition = new();

            for (int i = 0; i < _weatherContainers.Count; i++)
            {
                VisualElement container = _weatherContainers[i];

                painter.BeginPath();
                Vector2 lineStart =
                    container.layout.position + new Vector2(container.layout.width / 2, 0);
                Vector2 lineEnd = lineStart + new Vector2(0, -container.layout.height / 4);
                painter.MoveTo(lineStart);
                painter.LineTo(lineEnd);
                painter.Stroke();

                if (i == 0)
                    firstPosition = lineEnd;
                else if (i == _weatherContainers.Count - 1)
                    lastPosition = lineEnd;
            }

            Rect rect = mission.WeatherLabelContainer.ChangeCoordinatesTo(this, contentRect);
            Vector2 weatherLabelContainer6OClock =
                rect.position
                + new Vector2(
                    mission.WeatherLabelContainer.resolvedStyle.width / 2,
                    mission.WeatherLabelContainer.resolvedStyle.height
                );
            painter.BeginPath();
            painter.MoveTo(weatherLabelContainer6OClock);
            painter.LineTo(weatherLabelContainer6OClock + new Vector2(0, -firstPosition.y));
            painter.Stroke();

            painter.BeginPath();
            painter.MoveTo(firstPosition);
            painter.LineTo(lastPosition);
            painter.Stroke();
        };
    }
}
