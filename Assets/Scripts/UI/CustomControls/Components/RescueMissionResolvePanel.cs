using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RescueMissionResolvePanel : VisualElement
{
    private readonly RescueMission _mission;

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

        RefreshUi();
    }

    public void RefreshUi()
    {
        Clear();

        // deployed mission ui
        DeployedMissionUi deployedMissionUi = new(_mission);
        deployedMissionUi.resolveButton.visible = false;
        Add(deployedMissionUi);

        // passenger list
        VisualElement passengerListContainer = new()
        {
            style =
            {
                flexGrow = 1,
            }
        };
        Add(passengerListContainer);

        foreach(Passenger passenger in _mission.Passengers)
        {
            passengerListContainer.Add(new Label(passenger.Status.ToString()));
        }

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
        Button supplyButton = new() { text = "Supply" };
        Button crewButton = new() { text = "Crew" };
        Button ignoreButton = new() { text = "Ignore" };
        Button finishButton = new() 
        { 
            text = "Finish",
            style =
            {
                marginLeft = new StyleLength(StyleKeyword.Auto)
            }
        };

        bottomButtonsContainer.Add(supplyButton);
        bottomButtonsContainer.Add(crewButton);
        bottomButtonsContainer.Add(ignoreButton);
        bottomButtonsContainer.Add(finishButton);
        Add(bottomButtonsContainer);
    }
}
