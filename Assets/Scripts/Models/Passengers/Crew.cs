using System;
using UnityEngine.UIElements;

public class Crew : Passenger
{
    public const int MAX_ENDURANCE_LEVEL = 8;

    public bool isResting = false;
    public readonly Label deployedLabel = new();

    private Mission _deployedMission;

    private int _medicLevel = 1;
    private int _enduranceLevel = 1;

    public int MedicLevel
    {
        get => _medicLevel;
        set => _medicLevel = Math.Min(value, GameManager.MAX_UPGRADE_LEVEL);
    }
    public int EnduranceLevel
    {
        get => _enduranceLevel;
        set => _enduranceLevel = Math.Min(value, MAX_ENDURANCE_LEVEL);
    }
    public int MedicLevelPercentage => (_medicLevel - 1) * 2;
    public int EnduranceLevelPercentage => (_enduranceLevel - 1) * 5;
    public Mission DeployedMission
    {
        get => _deployedMission;
        set
        {
            _deployedMission = value;
            deployedLabel.text = value is null ? "" : "(Deployed)";
        }
    }

    public Crew()
        : base()
    {
        ui.Add(deployedLabel);
    }

    protected override void OnClick(ClickEvent _)
    {
        Selected = !Selected;
        UiManager.Instance.GameplayScreen.crewSelectionPanel.OnCrewSelectChange();
    }
}
