using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TrainUpgradePanel : VisualElement
{
    public TrainUpgradePanel()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(TrainUpgradePanel)}.");
    }

    public TrainUpgradePanel(Train train)
    {
        Add(new Label(train.trainSO.name));
        Add(
            new UpgradeInterface(
                "Cart",
                100,
                train.CartLevel,
                $"Adds a cart to your train, will earn {UpgradeInterface.PLACEHOLDER}x more citizens, resources, supplies, and crew members",
                train.CartLevel.ToString(),
                () =>
                {
                    train.CartLevel++;
                    return (train.CartLevel, train.CartLevel.ToString());
                }
            )
        );
        Add(
            new UpgradeInterface(
                "Speed",
                100,
                train.SpeedLevel,
                $"Each time you skip an interval, there is a {UpgradeInterface.PLACEHOLDER}% chance of you skipping a second interval immediately",
                (train.SpeedLevel - 1).ToString(),
                () =>
                {
                    train.SpeedLevel++;
                    return (train.SpeedLevel, (train.SpeedLevel - 1).ToString());
                }
            )
        );
        Add(
            new UpgradeInterface(
                "Warmth",
                100,
                train.WarmthLevel,
                $"Decreases all weather effect by {UpgradeInterface.PLACEHOLDER}%",
                (train.WarmthLevel - 1).ToString(),
                () =>
                {
                    train.WarmthLevel++;
                    return (train.WarmthLevel, (train.WarmthLevel - 1).ToString());
                }
            )
        );

        Button backButton = new() { text = "Back" };
        backButton.clicked += () =>
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.trainList
            );

        Add(backButton);
    }
}
