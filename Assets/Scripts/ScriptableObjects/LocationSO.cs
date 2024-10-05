using UnityEngine;

[CreateAssetMenu(fileName = "LocationSO", menuName = "Scriptable Objects/Location")]
public class LocationSO : ScriptableObject
{
    public new string name;
    public int milesToNextStop;
}
