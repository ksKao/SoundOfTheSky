using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RescueMissionResolvePanel : VisualElement
{
    private readonly RescueMission _mission;
    private DeployedMissionUi _deployedMissionUi;
    private readonly VisualElement _passengerListContainer = new();

    public RescueMissionResolvePanel()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(RescueMissionResolvePanel)}");
    }

    public RescueMissionResolvePanel(RescueMission mission)
    {
        _mission = mission;

        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);

        _deployedMissionUi = new(mission);
        Add(_deployedMissionUi);

        // passenger list
        _passengerListContainer.style.flexGrow = 1;
        Add(_passengerListContainer);

        // bottom buttons
        VisualElement bottomButtonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                width = UiUtils.GetLengthPercentage(100),
                backgroundColor = Color.red
            }
        };
        Button supplyButton = new() { text = $"Supply {_mission.NumberOfSupplies}" };
        Button crewButton = new() { text = $"Crew {_mission.NumberOfCrews}" };
        Button ignoreButton = new() { text = "Ignore" };
        Button finishButton = new() 
        { 
            text = "Finish",
            style =
            {
                marginLeft = new StyleLength(StyleKeyword.Auto)
            },
        };
        finishButton.SetEnabled(false);

        bottomButtonsContainer.Add(supplyButton);
        bottomButtonsContainer.Add(crewButton);
        bottomButtonsContainer.Add(ignoreButton);
        bottomButtonsContainer.Add(finishButton);
        Add(bottomButtonsContainer);

        RegisterCallback<AttachToPanelEvent>(OnAttach);
    }

    ~RescueMissionResolvePanel()
    {
        UnregisterCallback<AttachToPanelEvent>(OnAttach);
    }

    private void OnAttach(AttachToPanelEvent e)
    {
        RegenerateDeployedMissionUi();

        _passengerListContainer.Clear();
        foreach(Passenger passenger in _mission.Passengers)
        {
            _passengerListContainer.Add(passenger.passengerUi);
        }
    }

    public void RegenerateDeployedMissionUi()
    {
        // deployed mission ui
        if (_deployedMissionUi is not null) Remove(_deployedMissionUi);

        _deployedMissionUi = new(_mission);
        _deployedMissionUi.resolveButton.visible = false;
        Add(_deployedMissionUi);
        _deployedMissionUi.SendToBack();
    }
}
