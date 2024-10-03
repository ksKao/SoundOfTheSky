using UnityEngine;

[CreateAssetMenu(fileName = "WeatherSO", menuName = "Scriptable Objects/Weather")]
public class WeatherSO : ScriptableObject
{
    public new readonly string name;
    public readonly double decisionMakingProbability; // probability that each interval will require player to make decision
    public readonly double rewardMultiplier;
}
