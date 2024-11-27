using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RescueMissionResolvePanel : VisualElement
{
    private readonly RescueMission _mission;
    private readonly VisualElement _passengerListContainer = new();
    private readonly Button _supplyButton = new();
    private readonly Button _crewButton = new();

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

        _supplyButton.clicked += () =>
            _mission.UseSupply(_mission.Passengers.Where(p => p.Selected).ToArray());

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
        RegisterCallback<DetachFromPanelEvent>(OnDetach);
    }

    public void RefreshSupplyAndCrewButtonText()
    {
        _supplyButton.text = $"Supply {_mission.NumberOfSupplies}";
        _crewButton.text = $"Crew {_mission.Crews.Length}";
    }

    private void OnAttach(AttachToPanelEvent e)
    {
        UiManager.Instance.GameplayScreen.deployedMissionList.Remove(_mission.DeployedMissionUi);
        Add(_mission.DeployedMissionUi);
        _mission.DeployedMissionUi.SendToBack();
        _mission.DeployedMissionUi.resolveButton.visible = false;

        _passengerListContainer.Clear();
        foreach (Passenger passenger in _mission.Passengers)
        {
            _passengerListContainer.Add(passenger.Ui);
        }

        RefreshSupplyAndCrewButtonText();
    }

    private void OnDetach(DetachFromPanelEvent e)
    {
        Remove(_mission.DeployedMissionUi);
        UiManager.Instance.GameplayScreen.deployedMissionList.Refresh();
    }
}
