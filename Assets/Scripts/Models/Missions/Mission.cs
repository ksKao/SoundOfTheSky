public abstract class Mission
{
    protected readonly WeatherSO _weather;
    protected readonly LocationSO _routeStartLocation;
    protected readonly LocationSO _routeEndLocation;

    public string MissionType => GetType().Name;
}
