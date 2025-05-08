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
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.fontSize = 24;

        VisualElement trainOverviewContainer = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture("train_upgrade_overview_background"),
                height = UiUtils.GetLengthPercentage(15),
                width = UiUtils.GetLengthPercentage(100),
                paddingTop = 25,
                paddingBottom = 25,
                paddingLeft = 25,
                paddingRight = 25,
                color = UiUtils.darkBlueTextColor,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
            },
        };

        trainOverviewContainer.Add(
            new Label()
            {
                text = train.trainSO.name.ToUpper(),
                style = { alignSelf = Align.FlexStart },
            }
        );

        trainOverviewContainer.Add(
            new Image() { sprite = train.trainSO.sprite, style = { width = 600, height = 100 } }
        );

        Add(trainOverviewContainer);

        VisualElement upgradeInterfaceContainer = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture("train_upgrade_page_panel"),
                flexGrow = 1,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                justifyContent = Justify.SpaceEvenly,
                alignItems = Align.FlexStart,
                marginTop = 20,
                marginBottom = 10,
                paddingLeft = 40,
                paddingRight = 40,
            },
        };

        Add(upgradeInterfaceContainer);

        upgradeInterfaceContainer.Add(
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
        upgradeInterfaceContainer.Add(
            new UpgradeInterface(
                "Speed",
                100,
                train.SpeedLevel,
                $"Each time you skip an interval, there is a {UpgradeInterface.PLACEHOLDER}% chance of you skipping a second interval immediately",
                train.SpeedLevelPercentage.ToString(),
                () =>
                {
                    train.SpeedLevel++;
                    return (train.SpeedLevel, train.SpeedLevelPercentage.ToString());
                }
            )
        );
        upgradeInterfaceContainer.Add(
            new UpgradeInterface(
                "Warmth",
                100,
                train.WarmthLevel,
                $"Decreases all weather effect by {UpgradeInterface.PLACEHOLDER}%",
                train.WarmthLevelPercentage.ToString(),
                () =>
                {
                    train.WarmthLevel++;
                    return (train.WarmthLevel, train.WarmthLevelPercentage.ToString());
                }
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
                height = 100,
            },
        };
        UiUtils.ToggleBorder(backButton, false);
        backButton.clicked += () =>
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.trainList
            );

        Add(backButton);
    }
}
