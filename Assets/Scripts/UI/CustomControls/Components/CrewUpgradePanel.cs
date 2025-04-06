using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CrewUpgradePanel : VisualElement
{
    public CrewUpgradePanel()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(TrainUpgradePanel)}.");
    }

    public CrewUpgradePanel(Crew crew)
    {
        Add(new Label(crew.Status.ToString()));

        Add(
            new UpgradeInterface(
                "Medic",
                100,
                crew.MedicLevel,
                $"When using crew member to heal a citizen, there is a {UpgradeInterface.PLACEHOLDER}% chance of changing the status up by 2 health status.",
                crew.MedicLevelPercentage.ToString(),
                () =>
                {
                    if (crew.isResting)
                        Debug.Log("Crew is currently resting.");
                    else if (crew.deployedMission is not null)
                        Debug.Log("Crew is currently on a mission");
                    else
                        crew.MedicLevel++;
                    return (crew.MedicLevel, crew.MedicLevelPercentage.ToString());
                }
            )
        );

        Add(
            new UpgradeInterface(
                "Endurance",
                100,
                crew.EnduranceLevel,
                $"During resupply mission, decrease chance of crew member getting sick by {UpgradeInterface.PLACEHOLDER}%.",
                crew.EnduranceLevelPercentage.ToString(),
                () =>
                {
                    if (crew.isResting)
                        Debug.Log("Crew is currently resting.");
                    else if (crew.deployedMission is not null)
                        Debug.Log("Crew is currently on a mission");
                    else
                        crew.EnduranceLevel++;
                    return (crew.EnduranceLevel, crew.EnduranceLevelPercentage.ToString());
                },
                Crew.MAX_ENDURANCE_LEVEL
            )
        );

        Button backButton = new() { text = "Back" };
        backButton.clicked += UiManager.Instance.GameplayScreen.bottomNavigationBar.ShowCrewList;

        Add(backButton);
    }
}
