using UnityEngine;

[CreateAssetMenu(fileName = "TrainSO", menuName = "Scriptable Objects/Train")]
public class TrainSO : ScriptableObject
{
    public readonly new string name;
    public readonly LocationSO routeStartLocation;
    public readonly LocationSO routeEndLocation;
}
