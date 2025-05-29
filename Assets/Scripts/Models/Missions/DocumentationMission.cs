using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DocumentationMission : Mission
{
    private const int WEATHER_DISTRIBUTION_SUM = 10;

    private readonly NumberInput _resourceNumberInput = new("Resource");
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _paymentNumberInput = new("Payment");
    private readonly Label _undocumentedCitizenNumberLabel = new();
    private readonly Label _documentedCitizenNumberLabel = new();
    private readonly Label _resourceAmountLabel = new();
    private readonly Label _supplyAmountLabel = new();
    private readonly Label _paymentAmountLabel = new();
    private readonly Label _timeRemainingLabel = new("00:00");
    private int _initialCitizens = 0;

    public override int MilesPerInterval => 10;
    public float SecondsPassed { get; private set; } = 0;
    public override MissionType Type { get; } = MissionType.Documentation;
    public override Route Route { get; } = new(true);
    public int NumberOfResources { get; private set; } = 0;
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfPayments { get; private set; } = 0;
    public VisualElement WeatherLabelContainer { get; private set; } = new();
    public Dictionary<WeatherSO, int> WeatherProbabilities { get; private set; } =
        Random
            .DistributeNumbers(WEATHER_DISTRIBUTION_SUM, DataManager.Instance.AllWeathers.Length)
            .Select((prob, i) => new { index = i, prob })
            .ToDictionary((el) => DataManager.Instance.AllWeathers[el.index], el => el.prob);

    public override void Update()
    {
        base.Update();

        SecondsPassed += Time.deltaTime;
        _timeRemainingLabel.text = TimeSpan.FromSeconds(300 - SecondsPassed).ToString(@"mm\:ss");
    }

    public override bool Deploy()
    {
        if (Route.end.Residents <= 0)
        {
            UiUtils.ShowError(
                "Could not deploy documentation mission with no undocumented citizens."
            );
            return false;
        }

        if (GameManager.Instance.deployedMissions.Any(m => m.Type == Type))
        {
            UiUtils.ShowError("Another documentation mission has already been deployed.");
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

        _initialCitizens = Route.end.Citizens;

        GameManager.Instance.IncrementMaterialValue(MaterialType.Resources, -NumberOfResources);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Supplies, -NumberOfSupplies);
        GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, -NumberOfPayments);

        RerollWeather();
        UpdateLabels();

        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();
        DeployedMissionUi.routeLabel.text = Route.end.locationSO.name;

        DeployedMissionUi.materialLabelsContainer.Add(_undocumentedCitizenNumberLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_documentedCitizenNumberLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_resourceAmountLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_supplyAmountLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_paymentAmountLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_timeRemainingLabel);
        DeployedMissionUi.checkHealthButton.style.display = DisplayStyle.None;

        UpdateLabels();
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();

        AddRewardLabel(
            $"{Route.end.Citizens - _initialCitizens} New Citizen(s).",
            "reward_citizens"
        );
    }

    protected override void OnMileChange()
    {
        if (!IsMilestoneReached(MilesPerInterval))
            return;

        // max 5 minutes on documentation mission
        if (SecondsPassed >= 300)
        {
            Complete();
            return;
        }

        if (Route.end.Residents <= 0)
        {
            Complete();
            return;
        }

        int materialsConsumed = (int)
            Math.Round(
                weather.documentationMissionMaterialComsumptionMultiplier * (initialMiles / 5)
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

        Route.end.Residents--;
        Route.end.Citizens++;

        UpdateLabels();

        RerollWeather();
    }

    protected override void EventOccur() { }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        weatherUiInPendingMission.style.display = DisplayStyle.None;

        PendingMissionUi.style.position = Position.Relative;
        PendingMissionUi.RemoveAt(PendingMissionUi.childCount - 1);

        WeatherLabelContainer.Add(new Label("Weather"));

        PendingMissionUi.Add(WeatherLabelContainer);
        PendingMissionUi.Add(_resourceNumberInput);
        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_paymentNumberInput);

        PendingMissionUi.Add(new DocumentationMissionPendingWeatherTree(this));
    }

    protected override void ApplyCommonPendingMissionUiStyle()
    {
        for (int i = 0; i < PendingMissionUi.childCount - 1; i++)
        {
            ApplyCommonPendingMissionUiStyleSingle(PendingMissionUi.Children().ElementAt(i), i);
        }
    }

    // technically not used since documentation has no train, but override just in case
    protected override void ShowTrainList() { }

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

        DeployedMissionUi.weatherLabel.text = weather.name;
    }

    private bool CheckMaterials(
        NumberInput numberInput,
        string pluralMaterialName,
        MaterialType materialType
    )
    {
        if (numberInput.Value <= 0)
        {
            UiUtils.ShowError($"Please bring {pluralMaterialName} to continue");
            return false;
        }
        else if (numberInput.Value > GameManager.Instance.GetMaterialValue(materialType))
        {
            UiUtils.ShowError($"Not enough {pluralMaterialName} to deploy");
            return false;
        }

        return true;
    }

    private void UpdateLabels()
    {
        _undocumentedCitizenNumberLabel.text = $"Undocumented Citizens: {Route.end.Residents}";
        _documentedCitizenNumberLabel.text = $"Documented Citizens: {Route.end.Citizens}";
        _resourceAmountLabel.text = $"Resources: {NumberOfResources}";
        _supplyAmountLabel.text = $"Supplies: {NumberOfSupplies}";
        _paymentAmountLabel.text = $"Payments: {NumberOfPayments}";
    }
}
