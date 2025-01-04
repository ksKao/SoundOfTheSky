public class Location
{
    public readonly LocationSO locationSO;
    public int documentedCitizens = 0;
    public int undocumentedCitizens = 0;

    public Location(LocationSO locationSO)
    {
        this.locationSO = locationSO;
    }
}
