using UnityEngine;

[CreateAssetMenu(
    fileName = "CampaignModeWeatherSO",
    menuName = "Scriptable Objects/Campaign Mode Weather"
)]
public class CampaignModeWeatherSO : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public double changeOfWarming;
    public double eventChance;
    public int temperatureIncrease;
    public int temperatureDecrease;
}
