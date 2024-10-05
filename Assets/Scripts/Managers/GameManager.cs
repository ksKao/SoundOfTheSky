using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public const int NUMBER_OF_PENDING_MISSIONS_PER_TYPE = 5;

    public List<Mission> DeployedMissions { get; private set; } = new List<Mission>();
    public List<Mission> PendingMissions { get; private set; } = new List<Mission>();
    public GameplayScreen GameplayScreen { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        UIDocument uiDocument = FindFirstObjectByType<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogWarning("Could not find uiDocument object in scene.");
            GameplayScreen = new GameplayScreen();
        }
        else
        {
            GameplayScreen = uiDocument.rootVisualElement.Q<GameplayScreen>();
        }
    }


    private void OnEnable()
    {
        GameplayScreen.MissionTypeTab.TabView.activeTabChanged += OnMissionTabChanged;
    }

    private void Start()
    {
        RefreshMission<RescueMission>();
        RefreshMission<ResupplyMission>();
        RefreshMission<DocumentationMission>();
    }

    private void OnDisable()
    {
        GameplayScreen.MissionTypeTab.TabView.activeTabChanged -= OnMissionTabChanged;
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

    private void OnMissionTabChanged(Tab _, Tab selectedTab)
    {
        //VisualElement pendingMissionList = GameplayScreen.PendingMissionList
        if (Enum.TryParse(selectedTab.label, out MissionType selectedType))
        {
            GameplayScreen.PendingMissionList.Clear();

            foreach (Mission mission in PendingMissions)
            {
                if (mission.Type != selectedType) continue;

                GameplayScreen.PendingMissionList.Add(mission.GenerateMissionUI());
            }
        }
    }
}
