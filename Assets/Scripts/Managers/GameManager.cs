using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public const int NUMBER_OF_PENDING_MISSIONS_PER_TYPE = 5;
    private const int INITIAL_NUMBER_OF_CREWS = 5;

    private Mission _selectedPendingMission = null;
    private readonly Dictionary<AssetType, int> _assets = new()
    {
        { AssetType.Payments, 100 },
        { AssetType.Supplies, 100 },
        { AssetType.Resources, 100 },
        { AssetType.Citizens, 100 }
    };

    public readonly List<Mission> deployedMissions = new();
    public readonly List<Crew> crews = new(INITIAL_NUMBER_OF_CREWS);

    public List<Mission> PendingMissions { get; private set; } = new();

    public Mission SelectedPendingMission
    {
        get => _selectedPendingMission;
        set
        {
            UiManager.Instance.GameplayScreen.bottomNavigationBar.deployButton.visible = value is not null;

            // no need to do anything if selected the same one
            if (_selectedPendingMission == value) return;

            _selectedPendingMission?.OnDeselectMissionPendingUi();
            _selectedPendingMission = value;
        }
    }

    private void OnEnable()
    {
        UiManager.Instance.GameplayScreen.bottomNavigationBar.deployButton.clicked += DeploySelectedMission;
    }

    private void Start()
    {
        RefreshMission<RescueMission>();
        RefreshMission<ResupplyMission>();
        RefreshMission<DocumentationMission>();

        // need to update UI after generating the data
        UiManager.Instance.OnMissionActiveTabChange(new Tab(), UiManager.Instance.GameplayScreen.missionTypeTab.tabView.activeTab);
        UiManager.Instance.GameplayScreen.assetBar.RefreshAllAssetAmountUi();

        for (int i = 0; i < INITIAL_NUMBER_OF_CREWS; i++)
            crews.Add(new Crew());
    }

    private void Update()
    {
        foreach (Mission mission in deployedMissions)
            mission.Update();
    }

    private void OnDisable()
    {
        UiManager.Instance.GameplayScreen.bottomNavigationBar.deployButton.clicked -= DeploySelectedMission;
    }

    public void RefreshMission<T>() where T : Mission, new()
    {
        // do not allow passing in the Mission base class
        Type t = typeof(T);
        if (t == typeof(Mission))
        {
            Debug.LogWarning($"Detected calling {nameof(RefreshMission)} while passing in {nameof(Mission)}. This behavior is not allowed. Please pass in child classes instead.");
            return;
        }

        PendingMissions = PendingMissions.Where(m => m is not T).ToList();

        // can have 5 pending missions per mission type
        for (int i = 0; i < NUMBER_OF_PENDING_MISSIONS_PER_TYPE; i++)
        {
            PendingMissions.Add(new T());
        }
    }

    public int GetAssetValue(AssetType assetType)
    {
        if (!_assets.ContainsKey(assetType))
        {
            Debug.LogWarning($"Could not find asset type {assetType} in {nameof(_assets)}");
            return 0;
        }

        return _assets[assetType];
    }

    public void IncrementAssetValue(AssetType assetType, int number)
    {
        if (!_assets.ContainsKey(assetType))
        {
            Debug.LogWarning($"Could not find asset type {assetType} in {nameof(_assets)}");
            return;
        }

        _assets[assetType] += number;
        UiManager.Instance.GameplayScreen.assetBar.UpdateAssetAmount(assetType, _assets[assetType]);
    }

    private void DeploySelectedMission()
    {
        if (_selectedPendingMission is null)
        {
            Debug.LogWarning($"Could not deploy {nameof(_selectedPendingMission)}, variable is null");
            return;
        }

        if (!_selectedPendingMission.Deploy()) return;

        // move selected mission from pending to deployed
        PendingMissions.Remove(_selectedPendingMission);
        deployedMissions.Add(_selectedPendingMission);
        UiManager.Instance.GameplayScreen.deployedMissionList.Refresh();


        PendingMissions.Add((Mission) Activator.CreateInstance(_selectedPendingMission.GetType())); // replace current deployed mission with another one
        UiManager.Instance.RefreshMissionList(_selectedPendingMission.Type);
        _selectedPendingMission = null;
    }
}
