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
                "Adds a cart to your train, carry more citizens, resources, supplies, and crew members",
                () =>
                {
                    train.CartLevel++;
                    return train.CartLevel;
                }
            )
        );
        Add(
            new UpgradeInterface(
                "Speed",
                100,
                train.SpeedLevel,
                "Each time you skip an interval, there is a chance of you skipping a second interval immediately",
                () =>
                {
                    train.SpeedLevel++;
                    return train.SpeedLevel;
                }
            )
        );
        Add(
            new UpgradeInterface(
                "Warmth",
                100,
                train.WarmthLevel,
                "Decreases all weather effects",
                () =>
                {
                    train.WarmthLevel++;
                    return train.WarmthLevel;
                }
            )
        );
    }
}
