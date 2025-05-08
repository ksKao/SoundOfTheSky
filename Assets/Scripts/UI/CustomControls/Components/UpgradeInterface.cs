using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UpgradeInterface : VisualElement
{
    public const string PLACEHOLDER = "[placeholder]";

    private static readonly Texture2D _plusButtonBackground = UiUtils.LoadTexture(
        "upgrade_plus_button"
    );
    private static readonly Texture2D _upgradeSegmentEmpty = UiUtils.LoadTexture(
        "upgrade_segment_empty"
    );
    private static readonly Texture2D _upgradeSegmentFilled = UiUtils.LoadTexture(
        "upgrade_segment_filled"
    );

    private readonly Button _upgradeButton = new()
    {
        style =
        {
            backgroundImage = _plusButtonBackground,
            width = 40,
            height = 40,
            color = Color.white,
            backgroundColor = Color.clear,
        },
    };
    private readonly Label _label = new();
    private readonly Label _descriptionLabel = new();
    private readonly List<VisualElement> _progressBarSegments = new();
    private readonly string _labelString = "";
    private readonly string _descriptionString = "";
    private readonly int _maxLevel = 0;
    private int _upgradeCost = 0;

    public UpgradeInterface()
    {
        Debug.LogWarning($"Detected calling the default constructor of {nameof(UpgradeInterface)}");
    }

    public UpgradeInterface(
        string labelString,
        int initialCost,
        int currentLevel,
        string description,
        string placeholderValue,
        Func<(int, string)> onUpgrade,
        int maxLevel = 10
    )
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.color = UiUtils.darkBlueTextColor;
        style.width = UiUtils.GetLengthPercentage(100);

        _maxLevel = maxLevel;
        _labelString = labelString.ToUpper();
        _descriptionString = description;
        _upgradeCost = (int)Math.Round(initialCost * Math.Pow(1.1, currentLevel - 1));

        UiUtils.ToggleBorder(_upgradeButton, false);
        _upgradeButton.clicked += () =>
        {
            if (GameManager.Instance.GetMaterialValue(MaterialType.Payments) < _upgradeCost)
            {
                UiUtils.ShowError("Not enough payments to upgrade");
                return;
            }

            GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, -_upgradeCost);

            (int newLevel, string newPlaceholderValue) = onUpgrade();
            _upgradeCost = (int)Math.Round(_upgradeCost * 1.1);

            FormatLabel(newLevel);

            // hide upgrade button if is at max level
            if (newLevel >= maxLevel)
                _upgradeButton.visible = false;

            for (int i = 0; i < _progressBarSegments.Count; i++)
            {
                _progressBarSegments[i].style.backgroundImage =
                    i < newLevel ? _upgradeSegmentFilled : _upgradeSegmentEmpty;
            }

            RefreshDescriptionText(newPlaceholderValue);
        };

        VisualElement labelContainer = new();
        labelContainer.style.display = DisplayStyle.Flex;
        labelContainer.style.flexDirection = FlexDirection.Row;
        labelContainer.style.alignItems = Align.Center;
        labelContainer.style.marginBottom = 10;

        FormatLabel(currentLevel);
        labelContainer.Add(_label);
        labelContainer.Add(_upgradeButton);

        Add(labelContainer);

        VisualElement segmentContainer = new()
        {
            style = { display = DisplayStyle.Flex, flexDirection = FlexDirection.Row },
        };
        VisualElement[] segments = new VisualElement[maxLevel];

        for (int i = 0; i < maxLevel; i++)
        {
            segments[i] = new()
            {
                style =
                {
                    height = 30,
                    width = 45,
                    marginRight = 4,
                    backgroundImage =
                        i < currentLevel ? _upgradeSegmentFilled : _upgradeSegmentEmpty,
                },
            };
            segmentContainer.Add(segments[i]);
        }

        _progressBarSegments.AddRange(segments);
        Add(segmentContainer);

        RefreshDescriptionText(placeholderValue);
        _descriptionLabel.style.whiteSpace = WhiteSpace.Normal;
        Add(_descriptionLabel);
    }

    private void FormatLabel(int currentLevel)
    {
        StringBuilder sb = new();

        sb.Append(_labelString);

        if (currentLevel < _maxLevel)
        {
            sb.Append(": $");
            sb.Append(_upgradeCost);
        }

        _label.text = sb.ToString();
    }

    private void RefreshDescriptionText(string placeholderValue)
    {
        _descriptionLabel.text = _descriptionString.Replace(PLACEHOLDER, placeholderValue);
    }
}
