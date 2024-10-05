using UnityEngine;

[CreateAssetMenu(fileName = "WeatherSO", menuName = "Scriptable Objects/Weather")]
public class WeatherSO : ScriptableObject
{
    public new string name;
    public double decisionMakingProbability; // probability that each interval will require player to make decision
    public double rewardMultiplier;
}
