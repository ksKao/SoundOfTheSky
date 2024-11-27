using UnityEngine.UIElements;

public class Crew : Passenger
{
    private Mission _deployedMission;
    private readonly Label _deployedLabel = new();

    public Mission DeployedMission
    {
        get => _deployedMission;
        set
        {
            _deployedMission = value;
            _deployedLabel.text = value is null ? "" : "(Deployed)";
        }
    }

    public Crew()
        : base()
    {
        Ui.Add(_deployedLabel);
    }

    protected override void OnClick(ClickEvent _)
    {
        Selected = !Selected;
        UiManager.Instance.GameplayScreen.crewSelectionPanel.OnCrewSelectChange();
    }
}
