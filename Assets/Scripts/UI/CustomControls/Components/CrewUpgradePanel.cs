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

        Button backButton = new() { text = "Back" };
        backButton.clicked += () =>
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.crewList
            );
        ;

        Add(backButton);
    }
}
