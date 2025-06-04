using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeScreen : VisualElement
{
    public readonly WeatherBar weatherBar = new();

    public CampaignModeScreen()
    {
        style.backgroundImage = UiUtils.LoadTexture("background", GameplayMode.CampaignMode);
        style.minHeight = UiUtils.GetLengthPercentage(100);
        style.position = Position.Relative;

        Add(new PassengersWindow());
        Add(weatherBar);
    }
}
