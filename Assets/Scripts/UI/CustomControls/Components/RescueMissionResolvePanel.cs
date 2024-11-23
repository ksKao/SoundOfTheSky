using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RescueMissionResolvePanel : VisualElement
{
    private readonly RescueMission _mission;
    private readonly VisualElement _passengerListContainer = new();
    private readonly Button _supplyButton = new();
    private readonly Button _crewButton = new();
    private DeployedMissionUi _deployedMissionUi;

    public readonly Button finishButton = new();
    public readonly Button ignoreButton = new();

    public RescueMissionResolvePanel()
    {
        Debug.LogWarning(
            $"Detected calling default constructor of {nameof(RescueMissionResolvePanel)}"
        );
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
        VisualElement bottomButtonsContainer =
            new()
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row,
                    width = UiUtils.GetLengthPercentage(100),
                },
            };

        _supplyButton.clicked += () => _mission.UseSupply(_mission.Passengers.ToArray());

        _crewButton.clicked += () => _mission.UseCrew();

        ignoreButton.text = "Ignore";
        ignoreButton.clicked += () => _mission.Ignore();

        finishButton.text = "Finish";
        finishButton.style.marginLeft = new StyleLength(StyleKeyword.Auto);
        finishButton.clicked += () => _mission.Finish();

        bottomButtonsContainer.Add(_supplyButton);
        bottomButtonsContainer.Add(_crewButton);
        bottomButtonsContainer.Add(ignoreButton);
        bottomButtonsContainer.Add(finishButton);
        Add(bottomButtonsContainer);

        RegisterCallback<AttachToPanelEvent>(OnAttach);
    }

    public void RegenerateDeployedMissionUi()
    {
        // deployed mission ui
        if (_deployedMissionUi is not null)
            Remove(_deployedMissionUi);

        _deployedMissionUi = new(_mission);
        _deployedMissionUi.resolveButton.visible = false;
        Add(_deployedMissionUi);
        _deployedMissionUi.SendToBack();
    }

    public void RefreshButtonText()
    {
        _supplyButton.text = $"Supply {_mission.NumberOfSupplies}";
        _crewButton.text = $"Crew {Crew.GetCrewInMission(_mission).Length}";
    }

    private void OnAttach(AttachToPanelEvent e)
    {
        RegenerateDeployedMissionUi();

        _passengerListContainer.Clear();
        foreach (Passenger passenger in _mission.Passengers)
        {
            _passengerListContainer.Add(passenger.Ui);
        }

        RefreshButtonText();
    }
}
