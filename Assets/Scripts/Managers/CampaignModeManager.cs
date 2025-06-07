public class CampaignModeManager : Singleton<CampaignModeManager>
{
    public const int MAX_DAYS = 243;
    public const int DAY_TRANSITION_DURATION = 5;
    public const int NUMBER_OF_FUTURE_WEATHER_SHOWED = 5;
    public const int NUMBER_OF_CREWS = 5;

    private void Start()
    {
        // UiManager.Instance.CampaignModeScreen.weatherBar.weatherBarIcons.Transition();
        UiManager.Instance.CampaignModeScreen.mainChoicesContainer.RefreshTab();
    }

    public void ApplyAction(ActionSO action) { }
}
