using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DeployedMissionUi : VisualElement
{
    private readonly VisualElement _arriveOverlay = new();

    public readonly Mission mission;
    public readonly Label milesRemainingLabel = new();
    public readonly Button resolveButton = new();
    public readonly VisualElement materialLabelsContainer = new();

    public DeployedMissionUi()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(DeployedMissionUi)}.");
    }

    public DeployedMissionUi(Mission mission)
    {
        this.mission = mission;

        style.position = Position.Relative;

        _arriveOverlay.style.position = Position.Absolute;
        _arriveOverlay.style.backgroundColor = new Color(0, 0, 0, 0.5f);
        _arriveOverlay.style.width = UiUtils.GetLengthPercentage(100);
        _arriveOverlay.style.height = UiUtils.GetLengthPercentage(100);
        _arriveOverlay.style.display = DisplayStyle.Flex;
        _arriveOverlay.style.justifyContent = Justify.Center;
        _arriveOverlay.style.alignItems = Align.Center;
        Add(_arriveOverlay);

        Label arriveLabel =
            new()
            {
                style = { fontSize = new StyleLength(48), color = Color.white },
                text = "Arrive!",
            };
        _arriveOverlay.Add(arriveLabel);
        _arriveOverlay.visible = false;

        Add(new Label(mission.Route.start.name + " " + mission.Route.end.name));

        if (mission.Train != null)
        {
            Add(new Label(mission.Train.name));
        }

        milesRemainingLabel.text = mission.MilesRemaining.ToString();
        Add(milesRemainingLabel);

        materialLabelsContainer.style.display = DisplayStyle.Flex;
        Add(materialLabelsContainer);

        resolveButton.text = "Resolve";
        resolveButton.visible = mission.EventPending;
        resolveButton.clicked += mission.OnResolveButtonClicked;
        Add(resolveButton);
    }

    public void Arrive()
    {
        _arriveOverlay.visible = true;
        _arriveOverlay.RegisterCallback<ClickEvent>(
            (_) =>
            {
                Clear();
                Add(mission.MissionCompleteUi);
            }
        );
    }
}
