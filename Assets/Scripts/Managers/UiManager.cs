using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
    public GameplayScreen GameplayScreen { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        UIDocument uiDocument = FindFirstObjectByType<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogWarning("Could not find UIDocument object in scene.");
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

    private void OnDisable()
    {
        GameplayScreen.MissionTypeTab.TabView.activeTabChanged -= OnMissionTabChanged;
    }

    public void OnMissionTabChanged(Tab _, Tab selectedTab)
    {
        if (Enum.TryParse(selectedTab.label, out MissionType selectedType))
        {
            GameplayScreen.PendingMissionList.Clear();

            foreach (Mission mission in GameManager.Instance.PendingMissions)
            {
                if (mission.Type != selectedType) continue;

                GameplayScreen.PendingMissionList.Add(mission.PendingMissionUi);
            }
        }
    }
}
