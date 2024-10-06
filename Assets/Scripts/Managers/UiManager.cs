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
            Debug.LogWarning($"Could not find {nameof(UIDocument)} object in scene.");
            GameplayScreen = new GameplayScreen();
        }
        else
        {
            GameplayScreen = uiDocument.rootVisualElement.Q<GameplayScreen>();
        }
    }

    private void OnEnable()
    {
        GameplayScreen.missionTypeTab.tabView.activeTabChanged += OnMissionActiveTabChange;
    }

    private void OnDisable()
    {
        GameplayScreen.missionTypeTab.tabView.activeTabChanged -= OnMissionActiveTabChange;
    }

    public void OnMissionActiveTabChange(Tab _, Tab selected)
    {
        if (Enum.TryParse(selected.label, out MissionType selectedType))
            RefreshMissionList(selectedType);
    }

    public void RefreshMissionList(MissionType selectedType)
    {
        GameplayScreen.pendingMissionList.Clear();

        foreach (Mission mission in GameManager.Instance.PendingMissions)
        {
            if (mission.Type != selectedType) continue;

            GameplayScreen.pendingMissionList.Add(mission.PendingMissionUi);
        }

        GameManager.Instance.SelectedPendingMission = null;
    }
}
