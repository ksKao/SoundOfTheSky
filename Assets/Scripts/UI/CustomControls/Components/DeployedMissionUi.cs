using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DeployedMissionUi : VisualElement
{
    public readonly Mission mission;
    public readonly Label milesRemainingLabel = new();
    public readonly Button resolveButton = new();

    public DeployedMissionUi()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(DeployedMissionUi)}.");
    }

    public DeployedMissionUi(Mission mission)
    {
        this.mission = mission;

        Add(new Label(mission.Route.start.name + " " + mission.Route.end.name));
        milesRemainingLabel.text = mission.MilesRemaining.ToString();
        Add(milesRemainingLabel);

        resolveButton.text = "Resolve";
        resolveButton.visible = mission.EventPending;
        resolveButton.clicked += mission.OnResolveButtonClicked;
        Add(resolveButton);
    }
}
