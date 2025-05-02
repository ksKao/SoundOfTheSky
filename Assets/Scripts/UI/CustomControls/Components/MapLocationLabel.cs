using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MapLocationLabel : VisualElement
{
    private readonly Sprite _citizensIcon = UiUtils.LoadSprite("citizens");

    public MapLocationLabel()
    {
        Debug.LogWarning($"Detected calling the default constructor of {nameof(MapLocationLabel)}");
    }

    public MapLocationLabel(Location location)
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = location.locationSO.mapPinReverse ? FlexDirection.RowReverse : FlexDirection.Row;
        style.position = Position.Absolute;
        style.translate = new Translate(UiUtils.GetLengthPercentage(-50), UiUtils.GetLengthPercentage(-50));
        style.top = UiUtils.GetLengthPercentage(location.locationSO.yMapPinOffsetPercentage);
        style.left = UiUtils.GetLengthPercentage(location.locationSO.xMapPinOffsetPercentage);

        VisualElement nameLabelContainer = new()
        {
            style =
            {
                backgroundColor = UiUtils.HexToRgb("#0172bd"),
                color = Color.white,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
                justifyContent = Justify.Center,
                width = 96,
                height = 32,
                marginRight = 8
            }
        };
        UiUtils.ToggleBorder(nameLabelContainer, true, Color.white);
        UiUtils.SetBorderWidth(nameLabelContainer, 1);

        Label nameLabel = new(location.locationSO.name.ToUpper());
        nameLabelContainer.Add(nameLabel);

        Image citizensIcon = new()
        {
            sprite = _citizensIcon,
            style =
            {
                width = 32,
                height = 32,
            }
        };

        Label numberOfCitizensLabel = new()
        {
            text = $"{location.Citizens}x",
            style =
            {
                color = UiUtils.HexToRgb("#1b1464"),
                fontSize = 24
            }
        };

        Add(nameLabelContainer);
        Add(citizensIcon);
        Add(numberOfCitizensLabel);
    }
}
