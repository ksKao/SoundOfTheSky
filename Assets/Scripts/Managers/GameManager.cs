using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
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
    public readonly List<Crew> crews = new(INITIAL_NUMBER_OF_CREWS);

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
            UiManager.Instance.GameplayScreen.bottomNavigationBar.deployButton.visible =
                value is not null;

            // no need to do anything if selected the same one
            if (_selectedPendingMission == value)
                return;

            Mission oldMission = _selectedPendingMission;

            _selectedPendingMission = value;
            OnSelectedPendingMissionChange.Invoke(oldMission, value);
        }
    }

    public Coroutine StartExternalCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void StopExternalCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    protected override void Awake()
    {
        base.Awake();
        Locations = DataManager.Instance.AllLocations.Select(l => new Location(l)).ToArray();
        Trains = DataManager.Instance.AllTrains.Select(t => new Train(t)).ToArray();
        Application.runInBackground = true;
    }

    private void OnEnable()
    {
        UiManager.Instance.GameplayScreen.bottomNavigationBar.deployButton.clicked +=
            DeploySelectedMission;
    }

    private void Start()
    {
        UiManager.Instance.GameplayScreen.map.Refresh();
        RefreshAllMissions();
        UiManager.Instance.GameplayScreen.materialBar.RefreshAllMaterialAmountUi();

        for (int i = 0; i < INITIAL_NUMBER_OF_CREWS; i++)
            crews.Add(new Crew());
    }

    private void Update()
    {
        for (int i = 0; i < deployedMissions.Count; i++)
            deployedMissions[i].Update();
    }

    private void OnDisable()
    {
        UiManager.Instance.GameplayScreen.bottomNavigationBar.deployButton.clicked -=
            DeploySelectedMission;
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

    public void RefreshAllMissions()
    {
        RefreshMission<RescueMission>(5);
        RefreshMission<ResupplyMission>(5);
        RefreshMission<DocumentationMission>(1);

        // need to update UI after generating the data
        UiManager.Instance.GameplayScreen.missionTypeTab.RefreshTabHighlight();
        UiManager.Instance.GameplayScreen.RefreshMissionList(
            UiManager.Instance.GameplayScreen.missionTypeTab.ActiveTab
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
        UiManager.Instance.GameplayScreen.materialBar.RefreshAllMaterialAmountUi();
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
        UiManager.Instance.GameplayScreen.deployedMissionList.Refresh();

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

        UiManager.Instance.GameplayScreen.RefreshMissionList(_selectedPendingMission.Type);
        UiManager.Instance.GameplayScreen.ChangeRightPanel(
            UiManager.Instance.GameplayScreen.deployedMissionList
        );
        _selectedPendingMission = null;
    }
}
