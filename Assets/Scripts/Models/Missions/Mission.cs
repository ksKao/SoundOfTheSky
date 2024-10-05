using UnityEngine.UIElements;

public abstract class Mission
{
    protected readonly WeatherSO _weather;
    protected LocationSO _routeStartLocation;
    protected LocationSO _routeEndLocation;

    public abstract MissionType Type { get; }

    protected Mission()
    {
        _weather = DataManager.Instance.GetRandomWeather();
    }

    public abstract void FillMissionUi(VisualElement parent);
}

public enum MissionType
{
    Rescue,
    Resupply,
    Documentation
}
