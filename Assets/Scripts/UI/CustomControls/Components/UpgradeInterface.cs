using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UpgradeInterface : VisualElement
{
    private readonly Button _upgradeButton = new();
    private readonly Label _label = new();
    private readonly List<VisualElement> _progressBarSegments = new();
    private readonly string _labelString = "";
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
        Func<int> onUpgrade,
        int maxLevel = 10
    )
    {
        _maxLevel = maxLevel;
        _labelString = labelString;
        _upgradeCost = (int)Math.Round(initialCost * Math.Pow(1.1, currentLevel - 1));

        _upgradeButton.text = "+";
        _upgradeButton.style.width = StyleKeyword.Auto;
        _upgradeButton.clicked += () =>
        {
            if (GameManager.Instance.GetMaterialValue(MaterialType.Payments) < _upgradeCost)
            {
                Debug.Log("Not enough payments to upgrade");
                return;
            }

            GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, -_upgradeCost);

            int newLevel = onUpgrade();
            _upgradeCost = (int)Math.Round(_upgradeCost * 1.1);

            FormatLabel(newLevel);

            // hide upgrade button if is at max level
            if (newLevel >= maxLevel)
                _upgradeButton.visible = false;

            for (int i = 0; i < _progressBarSegments.Count; i++)
            {
                _progressBarSegments[i].style.backgroundColor =
                    i < newLevel ? Color.yellow : new Color();
            }
        };

        VisualElement labelContainer = new();
        labelContainer.style.display = DisplayStyle.Flex;
        labelContainer.style.flexDirection = FlexDirection.Row;

        FormatLabel(currentLevel);
        labelContainer.Add(_label);
        labelContainer.Add(_upgradeButton);

        Add(labelContainer);

        VisualElement segmentContainer =
            new()
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row,
                    width = UiUtils.GetLengthPercentage(80),
                    height = 20,
                },
            };
        VisualElement[] segments = new VisualElement[maxLevel];

        for (int i = 0; i < maxLevel; i++)
        {
            segments[i] = new()
            {
                style =
                {
                    flexGrow = 1,
                    height = UiUtils.GetLengthPercentage(100),
                    marginRight = 4,
                    backgroundColor = i < currentLevel ? Color.yellow : new Color(),
                },
            };
            UiUtils.SetBorderWidth(segments[i], 1);
            UiUtils.ToggleBorder(segments[i], true);
            segmentContainer.Add(segments[i]);
        }

        _progressBarSegments.AddRange(segments);
        Add(segmentContainer);

        Add(new Label(description) { style = { whiteSpace = WhiteSpace.Normal } });
    }

    private void FormatLabel(int currentLevel)
    {
        StringBuilder sb = new();

        sb.Append(_labelString);
        sb.Append(": ");

        if (currentLevel < _maxLevel)
            sb.Append(_upgradeCost);

        _label.text = sb.ToString();
    }
}
