using UnityEngine.UIElements;

[UxmlElement]
public partial class DeployedMissionList : VisualElement
{
    public DeployedMissionList()
    {
        Refresh();
    }

    public void Refresh()
    {
        // need this otherwise will have error since GameManager.Awake is called much later than the constructor.
        if (GameManager.Instance == null) return;

        Clear();

        if (GameManager.Instance.deployedMissions.Count == 0)
            UiManager.Instance.GameplayScreen.ChangeRightPanel(UiManager.Instance.GameplayScreen.map);

        foreach (Mission mission in GameManager.Instance.deployedMissions)
            Add(mission.DeployedMissionUi);
    }
}
