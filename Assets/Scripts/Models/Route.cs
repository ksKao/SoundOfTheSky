using System.Linq;

public struct Route
{
    public Location start;
    public Location end;

    public Route(LocationSO start, LocationSO end)
    {
        this.start = GameManager.Instance.Locations.FirstOrDefault(l =>
            l.locationSO.name == start.name
        );
        this.end = GameManager.Instance.Locations.FirstOrDefault(l =>
            l.locationSO.name == end.name
        );
    }
}
