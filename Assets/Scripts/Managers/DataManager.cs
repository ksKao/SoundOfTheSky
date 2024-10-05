using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private TrainSO[] _allTrains;
    [SerializeField] private WeatherSO[] _allWeathers;

    public TrainSO GetRandomTrain()
    {
        return GetRandomFromArray(_allTrains);
    }

    public WeatherSO GetRandomWeather()
    {
        return GetRandomFromArray(_allWeathers);
    }

    private T GetRandomFromArray<T>(T[] arr)
    {
        return arr[new System.Random().Next(arr.Length)];
    }
}
