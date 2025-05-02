using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Map : VisualElement
{
    private readonly Texture2D _mapImage = UiUtils.LoadTexture("map");

    public Map()
    {
        style.backgroundImage = _mapImage;
        style.marginLeft = UiUtils.GetLengthPercentage(2);
        style.marginRight = UiUtils.GetLengthPercentage(2);
        style.marginTop = UiUtils.GetLengthPercentage(2);
        style.marginBottom = UiUtils.GetLengthPercentage(2);
        style.position = Position.Relative;

        RegisterCallback<AttachToPanelEvent>((e) => Refresh());
    }

    public void Refresh()
    {
        if (GameManager.Instance == null) return;
        Clear();

        foreach (Location location in GameManager.Instance.Locations)
        {
            Add(new MapLocationLabel(location));
        }
    }
}
