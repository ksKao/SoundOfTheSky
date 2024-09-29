using UnityEngine;

[CreateAssetMenu(fileName = "LocationSO", menuName = "Scriptable Objects/Locations")]
public class LocationSO : ScriptableObject
{
    public new readonly string name;
    public readonly int milesToNextStop;
}
