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
    public List<DeployedRescueMissionSerializable> deployedRescueMissions = new();
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
    public string trainName;
    public int position; // used to re-order deployed missions, must be unique across deployed rescue, resupply, and documentation mission
    public List<PassengerSerializable> passengers = new();
    public int milesRemaining;
    public float secondsRemainingUntilNextMile;
    public List<string> crewIds;
    public List<string> crewIdsOnCooldown;
    public bool eventPending;
    public bool skippedLastInterval;
    public bool isCompleted;
    public int numberOfSupplies;
    public int numberOfResources;
    public int numberOfDeaths;
    public int numberOfResidents;
    public int numberOfNewResources;
    public bool actionTakenDuringThisEvent;
    public int deployedMissionStyleIndex;
    public MissionStatus status;
}
