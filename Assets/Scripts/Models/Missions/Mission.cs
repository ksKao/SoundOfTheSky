using UnityEngine.UIElements;

public abstract class Mission
{
    protected readonly WeatherSO _weather;

    public abstract MissionType Type { get; }
    public VisualElement PendingMissionUi { get; } = new();
    protected abstract (LocationSO start, LocationSO end) Route { get; }

    protected Mission()
    {
        _weather = DataManager.Instance.GetRandomWeather();
        GeneratePendingMissionUi();

        // after finish generating UI, make sure the elements are evenly spaced
        foreach (VisualElement child in PendingMissionUi.Children())
        {
            child.style.flexGrow = 1;
        }
    }

    protected virtual void GeneratePendingMissionUi()
    {
        PendingMissionUi.style.height = UiUtils.GetLengthPercentage(100 / GameManager.NUMBER_OF_PENDING_MISSIONS_PER_TYPE);
        PendingMissionUi.style.display = DisplayStyle.Flex;
        PendingMissionUi.style.flexDirection = FlexDirection.Row;

        VisualElement routeElement = new();
        routeElement.Add(new Label(Route.start.name));
        routeElement.Add(new Label(Route.end.name));

        PendingMissionUi.Add(routeElement);

        VisualElement weatherElement = new();
        weatherElement.Add(new Label(_weather.name));
        weatherElement.Add(new Label(_weather.decisionMakingProbability * 100 + "%"));

        PendingMissionUi.Add(weatherElement);

        PendingMissionUi.Add(new Label(Type.ToString()));
    }
}

public enum MissionType
{
    Rescue,
    Resupply,
    Documentation
}
