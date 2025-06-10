using System;

[Serializable]
public class CampaignModeState
{
    public int day;
    public int temperature;
    public bool skippedToday;
    public bool transitioning;
    public CampaignModeWeatherSerializable[] futureWeathers;
    public PassengerStatus[] statuses;
    public int[] crewCooldowns;
}

[Serializable]
public class CampaignModeWeatherSerializable
{
    public string name;
    public bool hidden;
}
