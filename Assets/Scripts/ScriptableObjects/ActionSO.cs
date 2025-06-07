using UnityEngine;

[CreateAssetMenu(fileName = "MedicalActionSO", menuName = "Scriptable Objects/Action SO")]
public class ActionSO : ScriptableObject
{
    public new string name;
    public ActionType type;
    public int valueIncrease;
    public int crewsNeeded;
    public int cooldownMin;
    public int cooldownMax;
}
