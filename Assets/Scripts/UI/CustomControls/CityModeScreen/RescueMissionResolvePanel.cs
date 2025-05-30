using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RescueMissionResolvePanel : VisualElement
{
    private static readonly Texture2D _buttonBackground = UiUtils.LoadTexture(
        "crew_selection_button"
    );
    private static readonly Texture2D _panelBackground = UiUtils.LoadTexture(
        "resolve_passenger_container_panel_background"
    );

    private readonly RescueMission _mission;
    private readonly Button _supplyButton = new();
    private readonly VisualElement _passengerListContainer = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            flexWrap = Wrap.Wrap,
            width = UiUtils.GetLengthPercentage(100),
        },
    };
    private readonly VisualElement _crewListContainer = new()
    {
        style =
        {
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            flexWrap = Wrap.Wrap,
            width = UiUtils.GetLengthPercentage(100),
        },
    };
    private readonly VisualElement _divider = new()
    {
        style =
        {
            height = 2,
            width = UiUtils.GetLengthPercentage(98),
            marginLeft = UiUtils.GetLengthPercentage(1),
            marginRight = UiUtils.GetLengthPercentage(1),
            backgroundColor = Color.black,
            marginTop = 16,
            marginBottom = 16,
        },
    };

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

        VisualElement container = new()
        {
            style =
            {
                backgroundImage = _panelBackground,
                paddingTop = UiUtils.GetLengthPercentage(5),
                paddingBottom = UiUtils.GetLengthPercentage(9),
                paddingLeft = UiUtils.GetLengthPercentage(3),
                paddingRight = UiUtils.GetLengthPercentage(3),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                flexGrow = 1,
            },
        };
        // passenger list
        Add(container);

        ScrollView scrollView = new()
        {
            mode = ScrollViewMode.Vertical,
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                width = UiUtils.GetLengthPercentage(100),
            },
        };
        scrollView.Add(_passengerListContainer);
        scrollView.Add(_divider);
        scrollView.Add(_crewListContainer);

        container.Add(scrollView);
        Add(container);

        // bottom buttons
        VisualElement bottomButtonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                minHeight = 90,
                width = UiUtils.GetLengthPercentage(100),
            },
        };

        _supplyButton.clicked += () =>
            _mission.UseSupply(_mission.CrewsAndPassengers.Where(p => p.Selected).ToArray());

        ignoreButton.text = "IGNORE";
        ignoreButton.clicked += () => _mission.Ignore();

        finishButton.text = "FINISH";
        finishButton.style.marginLeft = new StyleLength(StyleKeyword.Auto);
        finishButton.clicked += () => _mission.Finish();

        bottomButtonsContainer.Add(_supplyButton);
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

    public void RefreshButtonText()
    {
        _supplyButton.text = $"SUPPLY\n{_mission.NumberOfSupplies}";
    }

    private void OnAttach(AttachToPanelEvent e)
    {
        UiManager.Instance.CityModeScreen.deployedMissionList.Remove(_mission.DeployedMissionUi);
        Add(_mission.DeployedMissionUi);
        _mission.DeployedMissionUi.SendToBack();
        _mission.DeployedMissionUi.resolveButton.visible = false;

        _divider.style.display =
            _mission.Passengers.Count == 0 || _mission.Crews.Length == 0
                ? DisplayStyle.None
                : DisplayStyle.Flex;

        _passengerListContainer.Clear();
        foreach (Passenger passenger in _mission.Passengers)
        {
            passenger.Selected = false;
            _passengerListContainer.Add(passenger.ui);
        }

        _crewListContainer.Clear();
        foreach (Crew crew in _mission.Crews)
        {
            crew.Selected = false;
            _crewListContainer.Add(crew.ui);

            if (_mission.CrewsOnCooldown.Contains(crew))
            {
                crew.bracketLabel.text = "(Cooldown)";
                crew.bracketLabel.style.display = DisplayStyle.Flex;
            }
            else
            {
                crew.bracketLabel.text = "";
                crew.bracketLabel.style.display = DisplayStyle.None;
            }
        }

        Crew.OnSelect += _mission.UseCrew;
        RefreshButtonText();
    }

    private void OnDetach(DetachFromPanelEvent e)
    {
        _mission.DeployedMissionUi.resolveButton.visible = _mission.EventPending;
        Remove(_mission.DeployedMissionUi);
        UiManager.Instance.CityModeScreen.deployedMissionList.Refresh();

        foreach (Crew crew in _mission.Crews)
        {
            crew.bracketLabel.text = "";
            crew.bracketLabel.style.display = DisplayStyle.None;
        }

        Crew.OnSelect -= _mission.UseCrew;
    }
}
