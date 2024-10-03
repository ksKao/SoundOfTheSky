using UnityEngine;

[CreateAssetMenu(fileName = "LocationSO", menuName = "Scriptable Objects/Location")]
public class LocationSO : ScriptableObject
{
    public new readonly string name;
    public readonly int milesToNextStop;
}
