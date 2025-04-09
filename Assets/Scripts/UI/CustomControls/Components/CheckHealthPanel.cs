using UnityEngine;
using UnityEngine.UIElements;

public class CheckHealthPanel : VisualElement
{
    private readonly VisualElement _passengersContainer = new()
    {
        style =
        {
            backgroundImage = UiUtils.LoadTexture("crew_selection_panel_background"),
            paddingTop = UiUtils.GetLengthPercentage(5),
            paddingBottom = UiUtils.GetLengthPercentage(5),
            paddingLeft = UiUtils.GetLengthPercentage(3),
            paddingRight = UiUtils.GetLengthPercentage(3),
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            flexWrap = Wrap.Wrap,
            flexGrow = 1
        }
    };
    private readonly Mission _mission;

    public CheckHealthPanel()
    {
        Debug.LogWarning($"Calling default constructor of {nameof(CheckHealthPanel)}.");
    }

    public CheckHealthPanel(Mission mission)
    {
        _mission = mission;

        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.height = UiUtils.GetLengthPercentage(100);

        VisualElement buttonContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
                width = UiUtils.GetLengthPercentage(100),
            }
        };

        Button backButton = new()
        {
            text = "BACK",
            style =
            {
                backgroundImage = UiUtils.LoadTexture("crew_selection_button"),
                color = Color.white,
                width = 100,
                height = 90,
                fontSize = 24,
                backgroundColor = Color.clear,
            }
        };
        UiUtils.ToggleBorder(backButton, false);
        backButton.clicked += () => UiManager.Instance.GameplayScreen.ChangeRightPanel(UiManager.Instance.GameplayScreen.deployedMissionList);

        buttonContainer.Add(backButton);

        Add(_passengersContainer);
        Add(buttonContainer);

        RegisterCallback<AttachToPanelEvent>(OnAttach);
        RegisterCallback<DetachFromPanelEvent>(OnDetach);
    }

    public void Refresh()
    {
        _passengersContainer.Clear();

        foreach (Passenger passenger in _mission.CrewsAndPassengers)
        {
            passenger.Selected = false;
            passenger.selectable = false;
            _passengersContainer.Add(passenger.ui);
        }
    }

    private void OnAttach(AttachToPanelEvent e)
    {
        Refresh();
    }

    private void OnDetach(DetachFromPanelEvent e)
    {
        foreach (Passenger passenger in _mission.CrewsAndPassengers)
            passenger.selectable = true;
    }
}
