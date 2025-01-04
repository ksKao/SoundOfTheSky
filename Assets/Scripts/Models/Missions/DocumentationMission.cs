using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DocumentationMission : Mission
{
    private const int WEATHER_DISTRIBUTION_SUM = 10;

    private readonly LocationSO _destination = GetDestination();
    private readonly NumberInput _resourceNumberInput = new("Resource");
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _paymentNumberInput = new("Payment");
    private readonly Label _weatherLabel = new();
    private readonly Label _undocumentedCitizenNumberLabel = new();
    private readonly Label _documentedCitizenNumberLabel = new();
    private readonly Label _resourceAmountLabel = new();
    private readonly Label _supplyAmountLabel = new();
    private readonly Label _paymentAmountLabel = new();
    private int _initialDocumentedCitizens = 0;

    public override int MilesPerInterval => 10;
    public override MissionType Type { get; } = MissionType.Documentation;
    public override TrainSO Train { get; } = null;
    public override Route Route => new(DataManager.Instance.AllLocations[0], _destination);
    public int NumberOfResources { get; private set; } = 0;
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfPayments { get; private set; } = 0;
    public VisualElement WeatherLabelContainer { get; private set; } = new();
    public Dictionary<WeatherSO, int> WeatherProbabilities { get; private set; } =
        Random
            .DistributeNumbers(WEATHER_DISTRIBUTION_SUM, DataManager.Instance.AllWeathers.Length)
            .Select((prob, i) => new { index = i, prob })
            .ToDictionary((el) => DataManager.Instance.AllWeathers[el.index], el => el.prob);

    public override bool Deploy()
    {
        if (Route.end.undocumentedCitizens <= 0)
        {
            Debug.Log("Could not deploy documentation mission with no undocumented citizens.");
            return false;
        }

        if (GameManager.Instance.deployedMissions.Any(m => m.Type == Type))
        {
            Debug.Log("Another documentation mission has already been deployed.");
            return false;
        }

        if (!CheckMaterials(_resourceNumberInput, "resources", MaterialType.Resources))
            return false;
        if (!CheckMaterials(_supplyNumberInput, "supplies", MaterialType.Supplies))
            return false;
        if (!CheckMaterials(_paymentNumberInput, "payments", MaterialType.Payments))
            return false;

        NumberOfResources = _resourceNumberInput.Value;
        NumberOfSupplies = _supplyNumberInput.Value;
        NumberOfPayments = _paymentNumberInput.Value;

        _initialDocumentedCitizens = Route.end.documentedCitizens;

        GameManager.Instance.IncrementMaterialValue(MaterialType.Resources, -NumberOfResources);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Supplies, -NumberOfSupplies);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, -NumberOfPayments);

        RerollWeather();

        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();
        DeployedMissionUi.routeLabel.text = Route.end.locationSO.name;
        DeployedMissionUi.Add(_weatherLabel);

        DeployedMissionUi.Add(_undocumentedCitizenNumberLabel);
        DeployedMissionUi.Add(_documentedCitizenNumberLabel);
        DeployedMissionUi.Add(_resourceAmountLabel);
        DeployedMissionUi.Add(_supplyAmountLabel);
        DeployedMissionUi.Add(_paymentAmountLabel);

        UpdateLabels();
    }

    public override void Complete()
    {
        // refund remaining materials
        GameManager.Instance.IncrementMaterialValue(MaterialType.Resources, NumberOfResources);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Supplies, NumberOfSupplies);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, NumberOfPayments);

        base.Complete();
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();

        MissionCompleteUi.Add(
            new Label(
                $"{Route.end.documentedCitizens - _initialDocumentedCitizens} new citizen(s)."
            )
        );
    }

    protected override void OnMileChange()
    {
        if (!IsMilestoneReached(MilesPerInterval))
            return;

        // reset miles remaining after each interval since there is no train moving
        milesRemaining = initialMiles;

        if (Route.end.undocumentedCitizens <= 0)
        {
            Complete();
            return;
        }

        int materialsConsumed = (int)
            Math.Round(
                weather.documentationMissionMaterialComsumptionMultiplier
                    * Route.end.undocumentedCitizens
            );

        if (
            NumberOfSupplies < materialsConsumed
            || NumberOfPayments < materialsConsumed
            || NumberOfResources < materialsConsumed
        )
        {
            Complete();
            return;
        }

        NumberOfSupplies -= materialsConsumed;
        NumberOfPayments -= materialsConsumed;
        NumberOfResources -= materialsConsumed;

        Route.end.undocumentedCitizens--;
        Route.start.documentedCitizens++;

        UpdateLabels();

        RerollWeather();
    }

    protected override void EventOccur() { }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        weatherUiInPendingMission.style.display = DisplayStyle.None;

        PendingMissionUi.style.position = Position.Relative;

        WeatherLabelContainer.Add(new Label("Weather"));

        PendingMissionUi.Add(WeatherLabelContainer);
        PendingMissionUi.Add(_resourceNumberInput);
        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_paymentNumberInput);

        PendingMissionUi.Add(new DocumentationMissionPendingWeatherTree(this));
    }

    private static LocationSO GetDestination()
    {
        Location location = null;

        // prevent infinite loop below
        if (GameManager.Instance.Locations.Count == 1)
        {
            Debug.LogWarning(
                $"{nameof(GameManager.Instance.Locations)}.Length is 1. Could not call {nameof(GetDestination)} with only 1 element."
            );
            return GameManager.Instance.Locations[0].locationSO;
        }

        Location[] eligibleLocations = GameManager
            .Instance.Locations.Where((l, i) => l.undocumentedCitizens > 0 && i != 0)
            .ToArray();

        // if there are no locations with undocumented citizens, then only get random from all locations
        if (eligibleLocations.Length == 0)
            eligibleLocations = GameManager.Instance.Locations.ToArray();

        // cannot pick the first location as destination
        while (location == GameManager.Instance.Locations[0] || location == null)
        {
            location = Random.GetFromArray(eligibleLocations);
        }

        return location.locationSO;
    }

    private void RerollWeather()
    {
        int randomNo = new System.Random().Next(1, WEATHER_DISTRIBUTION_SUM + 1);

        foreach (KeyValuePair<WeatherSO, int> weatherProbability in WeatherProbabilities)
        {
            if (randomNo - weatherProbability.Value <= 0)
            {
                weather = weatherProbability.Key;
                break;
            }

            randomNo -= weatherProbability.Value;
        }

        _weatherLabel.text = weather.name;
    }

    private bool CheckMaterials(
        NumberInput numberInput,
        string pluralMaterialName,
        MaterialType materialType
    )
    {
        if (numberInput.Value <= 0)
        {
            Debug.Log($"Please bring {pluralMaterialName} to continue");
            return false;
        }
        else if (numberInput.Value > GameManager.Instance.GetMaterialValue(materialType))
        {
            Debug.Log($"Not enough {pluralMaterialName} to deploy");
            return false;
        }

        return true;
    }

    private void UpdateLabels()
    {
        _undocumentedCitizenNumberLabel.text =
            $"Undocumented Citizens: {Route.end.undocumentedCitizens}";
        _documentedCitizenNumberLabel.text = $"Documented Citizens: {Route.end.documentedCitizens}";
        _resourceAmountLabel.text = $"Resources: {NumberOfResources}";
        _supplyAmountLabel.text = $"Supplies: {NumberOfSupplies}";
        _paymentAmountLabel.text = $"Payments: {NumberOfPayments}";
    }
}
