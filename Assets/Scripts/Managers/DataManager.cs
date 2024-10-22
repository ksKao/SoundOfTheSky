using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private TrainSO[] _allTrains;
    [SerializeField] private WeatherSO[] _allWeathers;
    [SerializeField] private LocationSO[] _allLocations;

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

    public LocationSO GetRandomDestinationLocation()
    {
        LocationSO location = null;

        // prevent infinite loop below
        if (_allLocations.Length == 1)
        {
            Debug.LogWarning($"{nameof(_allLocations)}.Length is 1. Could not {nameof(GetRandomDestinationLocation)} with only 1 element.");
            return _allLocations[0];
        }

        // cannot pick the first location as destination
        while (location == _allLocations[0] || location == null)
        {
            location = Random.GetFromArray(_allLocations);
        }

        return location;
    }
}
