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
        // need this otherwise will have error since CityModeManager.Awake is called much later than the constructor.
        if (CityModeManager.Instance == null)
            return;

        Clear();

        if (CityModeManager.Instance.deployedMissions.Count == 0)
            UiManager.Instance.CityModeScreen.ChangeRightPanel(
                UiManager.Instance.CityModeScreen.map
            );

        foreach (Mission mission in CityModeManager.Instance.deployedMissions)
            Add(mission.DeployedMissionUi);
    }
}
