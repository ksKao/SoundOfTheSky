using System.Linq;
using UnityEngine.UIElements;

public class Crew : Passenger
{
    private Mission _deployedMission;
    private Label _deployedLabel = new();

    public Mission DeployedMission
    {
        get => _deployedMission;
        set
        {
            _deployedMission = value;
            _deployedLabel.text = value is null ? "" : "(Deployed)";
        }
    }

    public Crew() : base()
    {
        Ui.Add(_deployedLabel);
    }

    public static Crew[] GetCrewInMission(Mission mission)
    {
        return GameManager.Instance.crews.Where(c => c._deployedMission == mission).ToArray();
    }

    protected override void OnClick(ClickEvent _)
    {
        Selected = !Selected;
        UiManager.Instance.GameplayScreen.crewSelectionPanel.OnCrewSelectChange();
    }
}
