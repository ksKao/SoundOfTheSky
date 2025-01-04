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
    public override Route Route => new(Train.routeStartLocation, Train.routeEndLocation);
    public int NumberOfNewSupplies { get; private set; } = 0;
    public int NumberOfSupplies { get; private set; } = 0;
    public int NumberOfPayments { get; private set; } = 0;
    public int NumberOfResources { get; private set; } = 0;

    public override bool Deploy()
    {
        // check if this train has already been deployed
        if (
            GameManager.Instance.deployedMissions.Any(m =>
                m.Train != null && m.Train.name == Train.name
            )
        )
        {
            Debug.Log("Train has already been deployed.");
            return false;
        }

        // check if crew has been deployed
        if (Crews.Any(c => c.DeployedMission is not null))
        {
            Debug.Log("One or more crew(s) has been deployed in another mission");
            return false;
        }

        // check if player has enough supply
        if (GameManager.Instance.GetMaterialValue(MaterialType.Supplies) < _supplyNumberInput.Value)
        {
            Debug.Log("Not enough supplies to deploy this mission");
            return false;
        }

        // check if player has enough resources
        if (
            GameManager.Instance.GetMaterialValue(MaterialType.Resources)
            < _resourceNumberInput.Value
        )
        {
            Debug.Log("Not enough resources to deploy this mission");
            return false;
        }

        NumberOfSupplies = _supplyNumberInput.Value;
        GameManager.Instance.IncrementMaterialValue(MaterialType.Supplies, -NumberOfSupplies);

        NumberOfResources = _resourceNumberInput.Value;
        GameManager.Instance.IncrementMaterialValue(MaterialType.Resources, -NumberOfResources);

        foreach (Crew crew in _crewSelectionPanelButton.SelectedCrews)
            crew.DeployedMission = this;

        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";
        _deployedMissionResourcesLabel.text = $"{NumberOfResources} resource(s)";

        return true;
    }

    public override void Complete()
    {
        double rewardMultiplier = 1 + weather.rewardMultiplier;
        NumberOfNewSupplies = (int)Mathf.Round((float)rewardMultiplier * NumberOfNewSupplies);
        NumberOfPayments = (int)Mathf.Round((float)rewardMultiplier * NumberOfPayments);

        GameManager.Instance.IncrementMaterialValue(
            MaterialType.Supplies,
            NumberOfNewSupplies + NumberOfSupplies
        );
        GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, NumberOfPayments);

        foreach (Crew crew in Crews)
            crew.DeployedMission = null;

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

        MissionCompleteUi.Add(new Label($"{NumberOfNewSupplies} new supplies!"));
        MissionCompleteUi.Add(new Label($"{NumberOfPayments} new payments!"));
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();

        if (IsMilestoneReached(MilesPerInterval))
        {
            NumberOfNewSupplies += 2;
            NumberOfPayments += 5;
        }
    }

    public override void OnResolveButtonClicked()
    {
        Crew[] selectedCrews = { };

        Button ignoreOrFinishButton = new() { text = "Ignore" };
        ignoreOrFinishButton.clicked += () =>
        {
            EventPending = false;
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.deployedMissionList
            );
        };

        Button useSupplyButton = new() { text = "Use Supply" };
        useSupplyButton.clicked += () =>
        {
            // check if player has selected crew(s)
            if (selectedCrews.Length == 0)
            {
                Debug.Log("Please select at least 1 crew to use supply on.");
                return;
            }

            // check if player has enough supply
            if (selectedCrews.Length > NumberOfSupplies)
            {
                Debug.Log("Not enough supplies");
                return;
            }

            foreach (Crew crew in selectedCrews)
            {
                crew.Selected = false;
                crew.MakeBetter();
            }

            NumberOfSupplies -= selectedCrews.Length;
            ignoreOrFinishButton.text = "Finish";

            EventPending = false;
        };

        VisualElement buttonContainer = new();
        buttonContainer.style.display = DisplayStyle.Flex;
        buttonContainer.Add(useSupplyButton);
        buttonContainer.Add(ignoreOrFinishButton);

        UiManager.Instance.GameplayScreen.crewSelectionPanel.Show(
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
            buttonContainer
        );
    }

    protected override void EventOccur()
    {
        // 50% chance for an event to occur
        // when an event occurs, will first check if crew member's health and go down or not
        // if yes, make the crew's health worse
        // else consume resources
        if (Random.ShouldOccur(0.5))
        {
            // get a random crew that is in one condition above death
            Crew randomCrew = Random.GetFromArray(
                Crews.Where(c => c.Status < PassengerStatus.Death - 1).ToArray()
            );

            // no crews or all crews is in worst condition
            if (randomCrew is null)
            {
                NumberOfResources -= Mathf.Min(5, NumberOfResources);

                // mission fail when run out of resources and crews
                if (NumberOfResources == 0)
                {
                    Debug.Log("Resupply mission failed");
                    GameManager.Instance.deployedMissions.Remove(this);
                    UiManager.Instance.GameplayScreen.deployedMissionList.Refresh();
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

        PendingMissionUi.Add(new Label(Train.name));

        PendingMissionUi.Add(_supplyNumberInput);
        PendingMissionUi.Add(_crewSelectionPanelButton);
        PendingMissionUi.Add(_resourceNumberInput);
    }
}
