using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ResupplyMission : Mission
{
    private readonly NumberInput _supplyNumberInput = new("Supply");
    private readonly NumberInput _resourceNumberInput = new("Resource");
    private readonly CrewSelectionPanelButton _crewSelectionPanelButton = new();
    private readonly Label _deployedMissionCrewLabel = new();
    private readonly Label _deployedMissionResourcesLabel = new();

    public override MissionType Type { get; } = MissionType.Resupply;
    public override Passenger[] CrewsAndPassengers => Crews;
    public override Route Route { get; set; } = new(false);
    public int NumberOfNewSupplies { get; private set; } = 0;
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfPayments { get; private set; } = 0;
    public int NumberOfResources { get; private set; } = 0;

    public ResupplyMission()
        : base() { }

    public ResupplyMission(PendingMissionSerializable pendingMissionSerializable)
        : this()
    {
        Route = new Route(
            pendingMissionSerializable.routeStart,
            pendingMissionSerializable.routeEnd
        );

        WeatherSO foundWeather = DataManager.Instance.AllWeathers.FirstOrDefault(w =>
            w.name == pendingMissionSerializable.weather
        );

        if (foundWeather)
            weatherSO = foundWeather;

        SetupUi();
    }

    public ResupplyMission(DeployedResupplyMissionSerializable deployedResupplyMissionSerializable)
        : base()
    {
        Route = new Route(
            deployedResupplyMissionSerializable.routeStart,
            deployedResupplyMissionSerializable.routeEnd
        );

        WeatherSO foundWeather = DataManager.Instance.AllWeathers.FirstOrDefault(w =>
            w.name == deployedResupplyMissionSerializable.weather
        );

        if (foundWeather)
            weatherSO = foundWeather;

        Train = CityModeManager.Instance.Trains.FirstOrDefault(t =>
            t.trainSO.name == deployedResupplyMissionSerializable.trainName
        );

        SecondsRemainingUntilNextMile =
            deployedResupplyMissionSerializable.secondsRemainingUntilNextMile;

        foreach (string crewId in deployedResupplyMissionSerializable.crewIds)
        {
            Crew found = CityModeManager.Instance.crews.Find(c => c.id == crewId);

            if (found is null)
                continue;

            found.deployedMission = this;
        }

        IsCompleted = deployedResupplyMissionSerializable.isCompleted;

        NumberOfSupplies = deployedResupplyMissionSerializable.numberOfSupplies;

        NumberOfResources = deployedResupplyMissionSerializable.numberOfResources;

        NumberOfNewSupplies = deployedResupplyMissionSerializable.numberOfNewSupplies;

        NumberOfPayments = deployedResupplyMissionSerializable.numberOfPayments;

        SkippedLastInterval = deployedResupplyMissionSerializable.skippedLastInterval;

        SetupUi();

        DeployedMissionUi.trainImage.sprite = Train.trainSO.sprite;

        MilesRemaining = deployedResupplyMissionSerializable.milesRemaining;

        DeployedMissionUi.StyleIndex =
            deployedResupplyMissionSerializable.deployedMissionStyleIndex;

        MissionStatus = deployedResupplyMissionSerializable.status;

        EventPending = deployedResupplyMissionSerializable.eventPending;

        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";
        _deployedMissionResourcesLabel.text = $"{NumberOfResources} resource(s)";

        if (MissionStatus == MissionStatus.Arrived)
            DeployedMissionUi.Arrive();
        else if (MissionStatus == MissionStatus.Completed)
        {
            GenerateMissionCompleteUi();
            DeployedMissionUi.Complete();
        }
    }

    public override bool Deploy()
    {
        if (Train is null)
        {
            UiUtils.ShowError("Please select a train before proceeding");
            return false;
        }

        if (!Train.unlocked)
        {
            UiUtils.ShowError("You must unlock this train first before deploying");
            return false;
        }

        if (
            CityModeManager.Instance.deployedMissions.Any(m =>
                m.Train != null && m.Train.trainSO.name == Train.trainSO.name
            )
        )
        {
            UiUtils.ShowError("Train has already been deployed.");
            return false;
        }

        // check if crew has been deployed
        if (_crewSelectionPanelButton.SelectedCrews.Any(c => c.deployedMission is not null))
        {
            UiUtils.ShowError("One or more crew(s) has been deployed in another mission");
            return false;
        }

        // check if player has enough supply
        if (
            CityModeManager.Instance.GetMaterialValue(MaterialType.Supplies)
            < _supplyNumberInput.Value
        )
        {
            UiUtils.ShowError("Not enough supplies to deploy this mission");
            return false;
        }

        // check if player has enough resources
        if (
            CityModeManager.Instance.GetMaterialValue(MaterialType.Resources)
            < _resourceNumberInput.Value
        )
        {
            UiUtils.ShowError("Not enough resources to deploy this mission");
            return false;
        }

        NumberOfSupplies = _supplyNumberInput.Value;
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Supplies, -NumberOfSupplies);

        NumberOfResources = _resourceNumberInput.Value;
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Resources, -NumberOfResources);

        foreach (Crew crew in _crewSelectionPanelButton.SelectedCrews)
            crew.deployedMission = this;

        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";
        _deployedMissionResourcesLabel.text = $"{NumberOfResources} resource(s)";

        MissionStatus = MissionStatus.Deployed;
        return true;
    }

    public override void Complete()
    {
        double rewardMultiplier = 1 + weatherSO.rewardMultiplier;
        NumberOfNewSupplies = (int)Mathf.Round((float)rewardMultiplier * NumberOfNewSupplies);
        NumberOfPayments = (int)Mathf.Round((float)rewardMultiplier * NumberOfPayments);

        NumberOfNewSupplies += Route.end.Citizens * 5;
        NumberOfPayments += Route.end.Citizens * 5;

        CityModeManager.Instance.IncrementMaterialValue(
            MaterialType.Supplies,
            NumberOfNewSupplies + NumberOfSupplies
        );
        CityModeManager.Instance.IncrementMaterialValue(MaterialType.Payments, NumberOfPayments);

        foreach (Crew crew in Crews)
            crew.deployedMission = null;

        base.Complete();
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();

        DeployedMissionUi.materialLabelsContainer.Add(_deployedMissionCrewLabel);
        DeployedMissionUi.materialLabelsContainer.Add(_deployedMissionResourcesLabel);
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();

        AddRewardLabel($"{NumberOfNewSupplies} Supplies!", "reward_supplies");
        AddRewardLabel($"{NumberOfPayments} Payments!", "reward_payments");
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();

        if (IsMilestoneReached(MilesPerInterval))
        {
            NumberOfNewSupplies += 2 * Train.CartLevel;
            NumberOfPayments += 5 * Train.CartLevel;
        }
    }

    public override void OnResolveButtonClicked()
    {
        Crew[] selectedCrews = { };

        Button ignoreOrFinishButton = new() { text = "IGNORE" };
        ignoreOrFinishButton.clicked += () =>
        {
            EventPending = false;
            UiManager.Instance.CityModeScreen.ChangeRightPanel(
                UiManager.Instance.CityModeScreen.deployedMissionList
            );
        };

        Button useSupplyButton = new() { text = $"SUPPLY\n{NumberOfSupplies}" };
        useSupplyButton.clicked += () =>
        {
            // check if player has selected crew(s)
            if (selectedCrews.Length == 0)
            {
                UiUtils.ShowError("Please select at least 1 crew to use supply on.");
                return;
            }

            // check if player has enough supply
            if (selectedCrews.Length > NumberOfSupplies)
            {
                UiUtils.ShowError("Not enough supplies");
                return;
            }

            foreach (Crew crew in selectedCrews)
            {
                crew.Selected = false;
                crew.MakeBetter();
            }

            NumberOfSupplies -= selectedCrews.Length;
            ignoreOrFinishButton.text = "FINISH";
            useSupplyButton.text = $"SUPPLY\n{NumberOfSupplies}";

            EventPending = false;
        };

        VisualElement buttonContainer = new();
        buttonContainer.style.display = DisplayStyle.Flex;
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.Add(useSupplyButton);
        buttonContainer.Add(ignoreOrFinishButton);

        UiManager.Instance.CityModeScreen.crewSelectionPanel.Show(
            Crews,
            (crews) =>
            {
                // deselect all comfortable crew
                foreach (Crew crew in crews)
                {
                    if (crew.Status == PassengerStatus.Comfortable)
                    {
                        crew.Selected = false;
                    }
                }

                selectedCrews = crews.Where(c => c.Selected).ToArray();
            },
            null,
            null,
            buttonContainer
        );
    }

    protected override void EventOccur()
    {
        // get a random crew that is in one condition above death
        Crew randomCrew = Random.GetFromArray(
            Crews.Where(c => c.Status < PassengerStatus.Death - 1).ToArray()
        );

        // (50 - (crew endurance level - 1) * 5)% chance for an event to occur
        // when an event occurs, will first check if crew member's health and go down or not
        // if yes, make the crew's health worse
        // else consume resources
        if (Random.ShouldOccur(0.5 - randomCrew.EnduranceLevelPercentage))
        {
            // no crews or all crews is in worst condition
            if (randomCrew is null)
            {
                NumberOfResources -= Mathf.Min(5, NumberOfResources);
                _deployedMissionResourcesLabel.text = $"{NumberOfResources} resource(s)";

                // mission fail when run out of resources and crews
                if (NumberOfResources == 0)
                {
                    UiUtils.ShowError("Resupply mission failed");
                    CityModeManager.Instance.deployedMissions.Remove(this);
                    UiManager.Instance.CityModeScreen.deployedMissionList.Refresh();
                }
            }
            else
            {
                randomCrew.MakeWorse();
            }
        }

        // skip this event if no crew is sick
        if (Crews.All(c => c.Status == PassengerStatus.Comfortable))
            EventPending = false;
    }

    protected override void GeneratePendingMissionUi()
    {
        base.GeneratePendingMissionUi();

        VisualElement labelWrapper = new();

        labelWrapper.Add(UiUtils.WrapLabel(_trainNameLabel));
        labelWrapper.RegisterCallback<ClickEvent>((e) => ShowTrainList());

        PendingMissionUi.Add(labelWrapper);

        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_crewSelectionPanelButton);
        PendingMissionUi.Add(_resourceNumberInput);
    }
}
