using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private TrainSO[] _allTrains;
    [SerializeField] private WeatherSO[] _allWeathers;
    [SerializeField] private LocationSO[] _allLocations;

    public LocationSO[] AllLocations => _allLocations;

    public TrainSO GetRandomTrain()
    {
        return GetRandomFromArray(_allTrains);
    }

    public WeatherSO GetRandomWeather()
    {
        return GetRandomFromArray(_allWeathers);
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
            location = GetRandomFromArray(_allLocations);
        }

        return location;
    }

    private T GetRandomFromArray<T>(T[] arr)
    {
        return arr[new System.Random().Next(arr.Length)];
    }
}
