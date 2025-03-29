using UnityEngine;

[CreateAssetMenu(fileName = "TrainSO", menuName = "Scriptable Objects/Train")]
public class TrainSO : ScriptableObject
{
    public new string name;
    public int price;
    public Sprite sprite;
    public LocationSO routeStartLocation;
    public LocationSO routeEndLocation;
}
