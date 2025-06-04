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
        style.height = UiUtils.GetLengthPercentage(25);
        style.top = UiUtils.GetLengthPercentage(4);
        style.left = UiUtils.GetLengthPercentage(2);
    }
}
