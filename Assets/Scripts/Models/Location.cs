public class Location
{
    public readonly LocationSO locationSO;

    private int _citizens = 0;
    private int _residents = 0;

    public int Citizens
    {
        get => _citizens;
        set
        {
            _citizens = value;
            UiManager.Instance.CityModeScreen.materialBar.RefreshAllMaterialAmountUi();
            UiManager.Instance.CityModeScreen.map.Refresh();
        }
    }
    public int Residents
    {
        get => _residents;
        set
        {
            _residents = value;
            UiManager.Instance.CityModeScreen.materialBar.RefreshAllMaterialAmountUi();
            UiManager.Instance.CityModeScreen.map.Refresh();
        }
    }

    public Location(LocationSO locationSO)
    {
        this.locationSO = locationSO;
    }
}
