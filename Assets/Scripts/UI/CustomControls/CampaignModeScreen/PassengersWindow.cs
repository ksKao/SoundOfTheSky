using UnityEngine.UIElements;

[UxmlElement]
public partial class PassengersWindow : VisualElement
{
    public PassengersWindow()
    {
        style.backgroundImage = UiUtils.LoadTexture(
            "passenger_window_background",
            GameplayMode.CampaignMode
        );
        style.position = Position.Absolute;
        style.width = UiUtils.GetLengthPercentage(20);
        style.height = UiUtils.GetLengthPercentage(80);
        style.top = 32;
        style.left = 32;
    }
}
