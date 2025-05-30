using System;
using System.Collections.Generic;

[Serializable]
public class CityModeState
{
    public int numberOfPayments = 0;
    public int numberOfSupplies = 0;
    public int numberOfResources = 0;
    public List<LocationSerializable> locations = new();
    public List<CrewSerializable> crews = new();
    public List<TrainSerializable> trains = new();
    public List<PendingMissionSerializable> pendingRescueAndResupplyMissions = new();
    public PendingDocumentationMissionSerializable pendingDocumentationMission;
}

[Serializable]
public class LocationSerializable
{
    public string name;
    public int numberOfResidents;
    public int numberOfCitizens;
}

[Serializable]
public class CrewSerializable
{
    public string id;
    public string name;
    public PassengerStatus status;
    public int missionIndex;
    public int medicLevel;
    public int enduranceLevel;
}

[Serializable]
public class TrainSerializable
{
    public string name;
    public bool unlocked;
    public int cartLevel;
    public int speedLevel;
    public int warmthLevel;
}

[Serializable]
public class PendingMissionSerializable
{
    public string routeStart;
    public string routeEnd;
    public string weather;
    public MissionType type;
}

[Serializable]
public class PendingDocumentationMissionSerializable
{
    public string routeEnd;
    public int[] weatherProbabilities;
}

[Serializable]
public class PassengerSerializable
{
    public string name;
    public PassengerStatus status;
}

[Serializable]
public class DeployedRescueMissionSerializable
{
    public string routeStart;
    public string routeEnd;
    public string weather;
    public List<PassengerSerializable> passengers;
    public int milesRemaining;
    public float secondsRemainingUntilNextMile;
    public List<string> crewIdsOnCooldown;
    public bool eventPending;
    public int numberOfSupplies;
    public int numberOfResources;
    public int numberOfDeaths;
    public string backgroundImage;
    public string status;
}
