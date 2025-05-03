using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RescueMissionResolvePanel : VisualElement
{
    private static readonly Texture2D _buttonBackground = UiUtils.LoadTexture("crew_selection_button");
    private static readonly Texture2D _panelBackground = UiUtils.LoadTexture("resolve_passenger_container_panel_background");

    private readonly RescueMission _mission;
    private readonly VisualElement _passengerListContainer = new()
    {
        style =
        {
            backgroundImage = _panelBackground,
            paddingTop = UiUtils.GetLengthPercentage(5),
            paddingBottom = UiUtils.GetLengthPercentage(5),
            paddingLeft = UiUtils.GetLengthPercentage(3),
            paddingRight = UiUtils.GetLengthPercentage(3),
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            flexWrap = Wrap.Wrap
        }
    };
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
            _mission.UseSupply(_mission.CrewsAndPassengers.Where(p => p.Selected).ToArray());

        _crewButton.clicked += () => _mission.UseCrew();

        ignoreButton.text = "IGNORE";
        ignoreButton.clicked += () => _mission.Ignore();

        finishButton.text = "FINISH";
        finishButton.style.marginLeft = new StyleLength(StyleKeyword.Auto);
        finishButton.clicked += () => _mission.Finish();

        bottomButtonsContainer.Add(_supplyButton);
        bottomButtonsContainer.Add(_crewButton);
        bottomButtonsContainer.Add(ignoreButton);
        bottomButtonsContainer.Add(finishButton);
        Add(bottomButtonsContainer);

        List<Button> buttons = bottomButtonsContainer.Query<Button>().ToList();
        foreach (Button button in buttons)
        {
            button.style.backgroundImage = _buttonBackground;
            button.style.color = Color.white;
            button.style.width = 100;
            button.style.height = 90;
            button.style.fontSize = 24;
            button.style.backgroundColor = Color.clear;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            UiUtils.ToggleBorder(button, false);
        }

        RegisterCallback<AttachToPanelEvent>(OnAttach);
        RegisterCallback<DetachFromPanelEvent>(OnDetach);
    }

    public void RefreshSupplyAndCrewButtonText()
    {
        _supplyButton.text = $"SUPPLY\n{_mission.NumberOfSupplies}";
        _crewButton.text = $"CREW\n{_mission.Crews.Length}";
    }

    private void OnAttach(AttachToPanelEvent e)
    {
        UiManager.Instance.GameplayScreen.deployedMissionList.Remove(_mission.DeployedMissionUi);
        Add(_mission.DeployedMissionUi);
        _mission.DeployedMissionUi.SendToBack();
        _mission.DeployedMissionUi.resolveButton.visible = false;

        _passengerListContainer.Clear();
        foreach (Passenger passenger in _mission.CrewsAndPassengers)
        {
            passenger.BackgroundStyle = PassengerBackgroundStyle.Gray;
            passenger.Selected = false;
            _passengerListContainer.Add(passenger.ui);
        }

        RefreshSupplyAndCrewButtonText();
    }

    private void OnDetach(DetachFromPanelEvent e)
    {
        _mission.DeployedMissionUi.resolveButton.visible = _mission.EventPending;
        Remove(_mission.DeployedMissionUi);
        UiManager.Instance.GameplayScreen.deployedMissionList.Refresh();
    }
}
