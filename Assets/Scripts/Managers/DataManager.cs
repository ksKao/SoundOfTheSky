using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField]
    private TrainSO[] _allTrains;

    [SerializeField]
    private WeatherSO[] _allWeathers;

    [SerializeField]
    private LocationSO[] _allLocations;

    public LocationSO[] AllLocations => _allLocations;
    public WeatherSO[] AllWeathers => _allWeathers;

    public TrainSO GetRandomTrain()
    {
        return Random.GetFromArray(_allTrains);
    }

    public WeatherSO GetRandomWeather()
    {
        return Random.GetFromArray(_allWeathers);
    }
}
