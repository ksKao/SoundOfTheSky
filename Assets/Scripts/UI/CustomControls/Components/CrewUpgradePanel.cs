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
        style.display = DisplayStyle.Flex;
        style.justifyContent = Justify.SpaceBetween;
        style.flexDirection = FlexDirection.Column;
        style.fontSize = 24;

        VisualElement upgradeInterfaceContainer = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture("crew_selection_panel_background"),
                // width = UiUtils.GetLengthPercentage(100),
                // flexGrow = 1,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                justifyContent = Justify.SpaceEvenly,
                alignItems = Align.FlexStart,
                marginTop = 20,
                marginBottom = 10,
                paddingTop = 120,
                paddingBottom = 120,
                paddingLeft = 40,
                paddingRight = 40
            }
        };

        upgradeInterfaceContainer.Add(
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

        upgradeInterfaceContainer.Add(new()
        {
            style = { height = 120 }
        });

        upgradeInterfaceContainer.Add(
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

        Button backButton = new()
        {
            text = "BACK",
            style =
            {
                backgroundImage = UiUtils.LoadTexture("back_button"),
                backgroundColor = Color.clear,
                alignSelf = Align.FlexEnd,
                color = Color.white,
                width = 120,
                height = 100
            }
        };
        backButton.clicked += UiManager.Instance.GameplayScreen.bottomNavigationBar.ShowCrewList;
        UiUtils.ToggleBorder(backButton, false);

        Add(upgradeInterfaceContainer);
        Add(backButton);
    }
}
