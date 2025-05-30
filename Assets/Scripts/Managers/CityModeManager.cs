using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Analytics;

public class CityModeManager : Singleton<CityModeManager>
{
    private const int INITIAL_NUMBER_OF_CREWS = 5;
    public const int MAX_UPGRADE_LEVEL = 10;
    public const int MAX_CREW_COUNT = 200;

    private Mission _selectedPendingMission = null;
    private readonly Dictionary<MaterialType, int> _materials = new()
    {
        { MaterialType.Payments, 100 },
        { MaterialType.Supplies, 100 },
        { MaterialType.Resources, 100 },
    };

    public readonly List<Mission> deployedMissions = new();
    public List<Crew> crews = new(INITIAL_NUMBER_OF_CREWS);

    public string cityModeSaveFilePath = "";
    public Location[] Locations { get; private set; } = { };
    public Train[] Trains { get; private set; } = { };
    public List<Mission> PendingMissions { get; private set; } = new();
    public float SecondsPerMile { get; set; } = Mission.DEFAULT_SECONDS_PER_MILE;
    public event Action<Mission, Mission> OnSelectedPendingMissionChange;
    public Mission SelectedPendingMission
    {
        get => _selectedPendingMission;
        set
        {
            UiManager.Instance.CityModeScreen.bottomNavigationBar.deployButton.visible =
                value is not null;

            // no need to do anything if selected the same one
            if (_selectedPendingMission == value)
                return;

            Mission oldMission = _selectedPendingMission;

            _selectedPendingMission = value;
            OnSelectedPendingMissionChange.Invoke(oldMission, value);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        cityModeSaveFilePath = Path.Combine(Application.persistentDataPath, "city_mode.json");
        Locations = DataManager.Instance.AllLocations.Select(l => new Location(l)).ToArray();
        Trains = DataManager.Instance.AllTrains.Select(t => new Train(t)).ToArray();
        Application.runInBackground = true;
    }

    private void OnEnable()
    {
        UiManager.Instance.CityModeScreen.bottomNavigationBar.deployButton.clicked +=
            DeploySelectedMission;
    }

    private void Start()
    {
        UiManager.Instance.CityModeScreen.map.Refresh();
        RefreshAllMissions();
        UiManager.Instance.CityModeScreen.materialBar.RefreshAllMaterialAmountUi();

        for (int i = 0; i < INITIAL_NUMBER_OF_CREWS; i++)
            crews.Add(new Crew());

        LoadGame();
    }

    private void Update()
    {
        for (int i = 0; i < deployedMissions.Count; i++)
            deployedMissions[i].Update();
    }

    private void OnDisable()
    {
        UiManager.Instance.CityModeScreen.bottomNavigationBar.deployButton.clicked -=
            DeploySelectedMission;
    }

    private void OnDestroy()
    {
        SaveGame();
    }

    public void RefreshAllMissions()
    {
        RefreshMission<RescueMission>(5);
        RefreshMission<ResupplyMission>(5);
        RefreshMission<DocumentationMission>(1);

        // need to update UI after generating the data
        UiManager.Instance.CityModeScreen.missionTypeTab.RefreshTabHighlight();
        UiManager.Instance.CityModeScreen.RefreshMissionList(
            UiManager.Instance.CityModeScreen.missionTypeTab.ActiveTab
        );
    }

    public int GetMaterialValue(MaterialType materialType)
    {
        if (!_materials.ContainsKey(materialType))
        {
            Debug.LogWarning(
                $"Could not find material type {materialType} in {nameof(_materials)}"
            );
            return 0;
        }

        return _materials[materialType];
    }

    public void IncrementMaterialValue(MaterialType materialType, int number)
    {
        if (!_materials.ContainsKey(materialType))
        {
            Debug.LogWarning(
                $"Could not find material type {materialType} in {nameof(_materials)}"
            );
            return;
        }

        if (_materials.ContainsKey(materialType))
            _materials[materialType] += number;
        UiManager.Instance.CityModeScreen.materialBar.RefreshAllMaterialAmountUi();
    }

    private void DeploySelectedMission()
    {
        if (_selectedPendingMission is null)
        {
            Debug.LogWarning(
                $"Could not deploy {nameof(_selectedPendingMission)}, variable is null"
            );
            return;
        }

        if (!_selectedPendingMission.Deploy())
            return;

        // move selected mission from pending to deployed
        PendingMissions.Remove(_selectedPendingMission);
        deployedMissions.Add(_selectedPendingMission);
        UiManager.Instance.CityModeScreen.deployedMissionList.Refresh();

        Mission cacheSelectedPendingMission = _selectedPendingMission; // need to do this, otherwise the instance will be null when accessed inside the DOTween setter
        DOTween.To(
            () => 0.5f,
            (x) =>
            {
                cacheSelectedPendingMission.DeployedMissionUi.style.scale = new Vector2(x, x);
            },
            1f,
            .4f
        );

        // check if pending mission has the same train selected as the deployed mission, if yes, set it back to null
        foreach (Mission mission in PendingMissions)
        {
            if (mission.Train == _selectedPendingMission.Train)
                mission.Train = null;
        }

        Mission newMission = (Mission)Activator.CreateInstance(_selectedPendingMission.GetType()); // replace current deployed mission with another one
        DOTween.To(
            () => 0.5f,
            (x) =>
            {
                newMission.PendingMissionUi.style.scale = new Vector2(x, x);
            },
            1f,
            .4f
        );
        PendingMissions.Add(newMission);

        UiManager.Instance.CityModeScreen.RefreshMissionList(_selectedPendingMission.Type);
        UiManager.Instance.CityModeScreen.ChangeRightPanel(
            UiManager.Instance.CityModeScreen.deployedMissionList
        );
        _selectedPendingMission = null;
    }

    private void RefreshMission<T>(int numberOfMissionsToRefresh)
        where T : Mission, new()
    {
        // do not allow passing in the Mission base class
        Type t = typeof(T);
        if (t == typeof(Mission))
        {
            Debug.LogWarning(
                $"Detected calling {nameof(RefreshMission)} while passing in {nameof(Mission)}. This behavior is not allowed. Please pass in child classes instead."
            );
            return;
        }

        PendingMissions = PendingMissions.Where(m => m is not T).ToList();

        // can have 5 pending missions per mission type
        for (int i = 0; i < numberOfMissionsToRefresh; i++)
        {
            PendingMissions.Add(new T());
        }
    }

    private void LoadGame()
    {
        if (!File.Exists(cityModeSaveFilePath))
            return;

        CityModeState savedData = null;
        try
        {
            string serialized = "";

            using (FileStream stream = new(cityModeSaveFilePath, FileMode.Open))
            {
                using (StreamReader reader = new(stream))
                {
                    serialized = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrWhiteSpace(serialized))
                return;

            savedData = JsonUtility.FromJson<CityModeState>(serialized);
        }
        catch (Exception e)
        {
            Debug.LogError("Error while reading saved data: " + e);
        }

        if (savedData is null)
            return;

        _materials[MaterialType.Payments] = savedData.numberOfPayments;
        _materials[MaterialType.Resources] = savedData.numberOfResources;
        _materials[MaterialType.Supplies] = savedData.numberOfSupplies;

        UiManager.Instance.CityModeScreen.materialBar.RefreshAllMaterialAmountUi();

        foreach (LocationSerializable location in savedData.locations)
        {
            Location found = Locations.FirstOrDefault(l => l.locationSO.name == location.name);

            if (found is null)
                continue;

            found.Citizens = location.numberOfCitizens;
            found.Residents = location.numberOfResidents;
        }

        crews = savedData.crews.Select(c => new Crew(c)).ToList();

        savedData.trains.ForEach(train =>
        {
            Train found = Trains.FirstOrDefault(t => t.trainSO.name == train.name);

            if (found is null)
                return;

            found.unlocked = train.unlocked;
            found.CartLevel = train.cartLevel;
            found.SpeedLevel = train.speedLevel;
            found.WarmthLevel = train.warmthLevel;
        });

        PendingMissions.Clear();

        foreach (
            PendingMissionSerializable pendingMissionSerializable in savedData.pendingRescueAndResupplyMissions
        )
        {
            if (pendingMissionSerializable.type == MissionType.Rescue)
                PendingMissions.Add(new RescueMission(pendingMissionSerializable));
            else if (pendingMissionSerializable.type == MissionType.Resupply)
                PendingMissions.Add(new ResupplyMission(pendingMissionSerializable));
        }

        PendingMissions.Add(new DocumentationMission(savedData.pendingDocumentationMission));

        UiManager.Instance.CityModeScreen.RefreshMissionList(
            UiManager.Instance.CityModeScreen.missionTypeTab.ActiveTab
        );

        deployedMissions.Clear();

        int numberOfDeployedMissions =
            savedData.deployedRescueMissions.Count + savedData.deployedResupplyMissions.Count;

        for (int i = 0; i < numberOfDeployedMissions; i++)
        {
            DeployedRescueMissionSerializable deployedRescueMissionSerializable =
                savedData.deployedRescueMissions.FirstOrDefault(m => m.order == i);
            DeployedResupplyMissionSerializable deployedResupplyMissionSerializable =
                savedData.deployedResupplyMissions.FirstOrDefault(m => m.order == i);

            if (deployedRescueMissionSerializable is not null)
                deployedMissions.Add(new RescueMission(deployedRescueMissionSerializable));
            else if (deployedResupplyMissionSerializable is not null)
                deployedMissions.Add(new ResupplyMission(deployedResupplyMissionSerializable));
        }

        UiManager.Instance.CityModeScreen.deployedMissionList.Refresh();
    }

    private void SaveGame()
    {
        CityModeState cityModeState = new()
        {
            numberOfPayments = GetMaterialValue(MaterialType.Payments),
            numberOfResources = GetMaterialValue(MaterialType.Resources),
            numberOfSupplies = GetMaterialValue(MaterialType.Supplies),
            locations = Locations
                .Select(l => new LocationSerializable()
                {
                    name = l.locationSO.name,
                    numberOfCitizens = l.Citizens,
                    numberOfResidents = l.Residents,
                })
                .ToList(),
            crews = crews
                .Select(c => new CrewSerializable()
                {
                    id = c.id,
                    name = c.Name,
                    enduranceLevel = c.EnduranceLevel,
                    medicLevel = c.MedicLevel,
                    status = c.Status,
                })
                .ToList(),
            trains = Trains
                .Select(t => new TrainSerializable()
                {
                    name = t.trainSO.name,
                    unlocked = t.unlocked,
                    cartLevel = t.CartLevel,
                    speedLevel = t.SpeedLevel,
                    warmthLevel = t.WarmthLevel,
                })
                .ToList(),
        };

        foreach (Mission mission in PendingMissions)
        {
            if (mission is RescueMission or ResupplyMission)
            {
                cityModeState.pendingRescueAndResupplyMissions.Add(
                    new()
                    {
                        routeStart = mission.Route.start.locationSO.name,
                        routeEnd = mission.Route.end.locationSO.name,
                        type = mission.Type,
                        weather = mission.WeatherSO.name,
                    }
                );
            }
            else if (mission is DocumentationMission documentationMission)
            {
                cityModeState.pendingDocumentationMission = new()
                {
                    routeEnd = documentationMission.Route.end.locationSO.name,
                    weatherProbabilities =
                        documentationMission.WeatherProbabilities.Values.ToArray(),
                };
            }
        }

        for (int i = 0; i < deployedMissions.Count; i++)
        {
            Mission mission = deployedMissions[i];
            if (mission is RescueMission rescueMission)
            {
                cityModeState.deployedRescueMissions.Add(
                    new()
                    {
                        routeStart = rescueMission.Route.start.locationSO.name,
                        routeEnd = rescueMission.Route.end.locationSO.name,
                        weather = rescueMission.WeatherSO.name,
                        passengers = rescueMission
                            .Passengers.Select(p => new PassengerSerializable()
                            {
                                name = p.Name,
                                status = p.Status,
                            })
                            .ToList(),
                        order = i,
                        trainName = rescueMission.Train.trainSO.name,
                        milesRemaining = rescueMission.MilesRemaining,
                        secondsRemainingUntilNextMile = rescueMission.SecondsRemainingUntilNextMile,
                        crewIds = rescueMission.Crews.Select(c => c.id).ToList(),
                        crewIdsOnCooldown = rescueMission
                            .CrewsOnCooldown.Select(c => c.id)
                            .ToList(),
                        eventPending = rescueMission.EventPending,
                        isCompleted = rescueMission.IsCompleted,
                        skippedLastInterval = rescueMission.SkippedLastInterval,
                        numberOfSupplies = rescueMission.NumberOfSupplies,
                        numberOfResources = rescueMission.NumberOfResources,
                        numberOfDeaths = rescueMission.NumberOfDeaths,
                        numberOfResidents = rescueMission.NumberOfResidents,
                        numberOfNewResources = rescueMission.NumberOfNewResources,
                        actionTakenDuringThisEvent = rescueMission.ActionTakenDuringThisEvent,
                        deployedMissionStyleIndex = rescueMission.DeployedMissionUi.StyleIndex,
                        status = rescueMission.MissionStatus,
                    }
                );
            }
            else if (mission is ResupplyMission resupplyMission)
            {
                cityModeState.deployedResupplyMissions.Add(
                    new()
                    {
                        routeStart = resupplyMission.Route.start.locationSO.name,
                        routeEnd = resupplyMission.Route.end.locationSO.name,
                        weather = resupplyMission.WeatherSO.name,
                        order = i,
                        trainName = resupplyMission.Train.trainSO.name,
                        milesRemaining = resupplyMission.MilesRemaining,
                        secondsRemainingUntilNextMile =
                            resupplyMission.SecondsRemainingUntilNextMile,
                        crewIds = resupplyMission.Crews.Select(c => c.id).ToList(),
                        eventPending = resupplyMission.EventPending,
                        isCompleted = resupplyMission.IsCompleted,
                        skippedLastInterval = resupplyMission.SkippedLastInterval,
                        numberOfSupplies = resupplyMission.NumberOfSupplies,
                        numberOfResources = resupplyMission.NumberOfResources,
                        numberOfNewSupplies = resupplyMission.NumberOfNewSupplies,
                        numberOfPayments = resupplyMission.NumberOfPayments,
                        deployedMissionStyleIndex = resupplyMission.DeployedMissionUi.StyleIndex,
                        status = resupplyMission.MissionStatus,
                    }
                );
            }
        }

        try
        {
            // create directory if it doesnt exist
            Directory.CreateDirectory(Path.GetDirectoryName(cityModeSaveFilePath));

            string serialized = JsonUtility.ToJson(cityModeState, true);

            using (FileStream stream = new(cityModeSaveFilePath, FileMode.Create))
            {
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(serialized);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error while saving data: " + e);
        }
    }
}
