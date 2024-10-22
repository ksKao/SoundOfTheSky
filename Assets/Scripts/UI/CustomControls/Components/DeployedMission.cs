using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DeployedMission : VisualElement
{
    public readonly Mission mission;
    public readonly Label milesRemainingLabel = new();

    public DeployedMission()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(DeployedMission)}.");
    }

    public DeployedMission(Mission mission)
    {
        this.mission = mission;

        Add(new Label(mission.Route.start.name + " " + mission.Route.end.name));
        milesRemainingLabel.text = mission.MilesRemaining.ToString();
        Add(milesRemainingLabel);
    }
}
