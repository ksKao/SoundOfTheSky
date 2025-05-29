using System;
using System.Linq;
using UnityEngine;

public readonly struct Route
{
    public readonly Location start;
    public readonly Location end;
    public readonly int startIndex;
    public readonly int endIndex;
    public readonly int distance;

    public Route(bool destinationMustContainResidents)
    {
        start = null;
        end = null;
        distance = 0;
        startIndex = 0;
        endIndex = 0;

        if (CityModeManager.Instance.Locations.Length == 0)
        {
            Debug.LogWarning(
                $"No locations found in {nameof(CityModeManager.Instance.Locations)}."
            );
        }

        // prevent infinite loop below
        if (CityModeManager.Instance.Locations.Length == 1)
        {
            Debug.LogWarning(
                $"{nameof(CityModeManager.Instance.Locations)}.Length is 1. Could not call {nameof(Route)} constructor with only 1 element."
            );

            start = CityModeManager.Instance.Locations[0];
            end = CityModeManager.Instance.Locations[0];
        }

        Location[] eligibleDestinations = { };

        if (destinationMustContainResidents)
        {
            Location[] eligibleLocations = CityModeManager
                .Instance.Locations.Where((l, i) => l.Residents > 0 && i != 0)
                .ToArray();

            // if there are no locations with undocumented citizens, then only get random from all locations
            if (eligibleLocations.Length == 0)
                eligibleLocations = CityModeManager.Instance.Locations.ToArray();
        }

        // cannot pick the first location as destination
        while (end == CityModeManager.Instance.Locations[0] || end == null)
        {
            // only get from all locations if there is no eligible destinations
            end = Random.GetFromArray(
                eligibleDestinations.Length > 0
                    ? eligibleDestinations
                    : CityModeManager.Instance.Locations
            );
        }

        endIndex = Array.IndexOf(CityModeManager.Instance.Locations, end);

        if (endIndex < 0)
        {
            Debug.LogWarning(
                "Error occured when generating route. End location is not found or at the first position."
            );
            return;
        }

        Location[] subArray = new Location[endIndex];
        Array.Copy(CityModeManager.Instance.Locations, subArray, endIndex);

        start = Random.GetFromArray(subArray);

        startIndex = Array.IndexOf(CityModeManager.Instance.Locations, start);

        for (int i = startIndex; i < endIndex; i++)
        {
            distance += DataManager.Instance.AllLocations[i].milesToNextStop;
        }
    }
}
