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

    public void RefreshMissionList(MissionType selectedType)
    {
        GameplayScreen.pendingMissionList.Clear();

        int i = 0;
        foreach (Mission mission in GameManager.Instance.PendingMissions)
        {
            if (mission.Type != selectedType) continue;

            GameplayScreen.pendingMissionList.Add(mission.PendingMissionUi);
            mission.PendingMissionUi.style.backgroundImage = Mission.pendingMissionBarBackground[i % Mission.pendingMissionBarBackground.Length];

            i++;
        }

        GameManager.Instance.SelectedPendingMission = null;
    }
}
