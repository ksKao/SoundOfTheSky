using UnityEngine;

[CreateAssetMenu(fileName = "TrainSO", menuName = "Scriptable Objects/Train")]
public class TrainSO : ScriptableObject
{
    public new string name;
    public LocationSO routeStartLocation;
    public LocationSO routeEndLocation;
}
