using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public const int NUMBER_OF_PENDING_MISSIONS_PER_TYPE = 5;

    private Mission _selectedPendingMission = null;

    public List<Mission> DeployedMissions { get; private set; } = new List<Mission>();
    public List<Mission> PendingMissions { get; private set; } = new List<Mission>();
    public Mission SelectedPendingMission
    {
        get => _selectedPendingMission;
        set
        {
            // no need to do anything if selected the same one
            if (_selectedPendingMission == value) return;

            _selectedPendingMission?.OnDeselectMissionPendingUi();
            _selectedPendingMission = value;
        }
    }

    private void Start()
    {
        RefreshMission<RescueMission>();
        RefreshMission<ResupplyMission>();
        RefreshMission<DocumentationMission>();

        // need to update UI after generating the data
        UiManager.Instance.OnMissionTabChanged(new Tab(), UiManager.Instance.GameplayScreen.MissionTypeTab.TabView.activeTab);
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
}
