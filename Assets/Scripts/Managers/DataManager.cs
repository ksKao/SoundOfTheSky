using System.Linq;
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
    public TrainSO[] AllTrains => _allTrains;

    private string[] NameList { get; set; }

    protected override void Awake()
    {
        base.Awake();

        TextAsset textAsset = Resources.Load<TextAsset>("Text/name_list");

        NameList = textAsset.text.Split('\n').ToArray();
    }

    public WeatherSO GetRandomWeather()
    {
        return Random.GetFromArray(_allWeathers);
    }

    public string GetRandomName()
    {
        return Random.GetFromArray(NameList);
    }
}
