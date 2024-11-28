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
    public int NumberOfSupplies { get; private set; } = 0;
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
        if (GameManager.Instance.GetAssetValue(AssetType.Supplies) < _supplyNumberInput.Value)
        {
            Debug.Log("Not enough supplies to deploy this mission");
            return false;
        }

        // check if player has enough resources
        if (GameManager.Instance.GetAssetValue(AssetType.Resources) < _resourceNumberInput.Value)
        {
            Debug.Log("Not enough resources to deploy this mission");
            return false;
        }

        NumberOfResources = _resourceNumberInput.Value;
        GameManager.Instance.IncrementAssetValue(AssetType.Resources, NumberOfResources);

        foreach (Crew crew in _crewSelectionPanelButton.SelectedCrews)
            crew.DeployedMission = this;

        _deployedMissionCrewLabel.text = $"{Crews.Length} crew(s)";
        _deployedMissionResourcesLabel.text = $"{NumberOfResources} resource(s)";

        return true;
    }

    public override void GenerateDeployedMissionUi()
    {
        base.GenerateDeployedMissionUi();

        DeployedMissionUi.assetLabelsContainer.Add(_deployedMissionCrewLabel);
        DeployedMissionUi.assetLabelsContainer.Add(_deployedMissionResourcesLabel);
    }

    public override void GenerateMissionCompleteUi()
    {
        base.GenerateMissionCompleteUi();
    }

    protected override void OnMileChange()
    {
        base.OnMileChange();

        if (IsMilestoneReached(MILES_PER_INTERVAL))
            NumberOfSupplies += 2;
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
        // 50% chance crew member's health will go up and down, crew member's health cannot go below sick in resupply mission
        foreach (Crew crew in Crews)
        {
            if (!Random.ShouldOccur(0.5))
                continue;

            // 50% to get better, 50% chance to get worse
            if (Random.ShouldOccur(0.5))
            {
                crew.MakeBetter();
            }
            else if ((int)crew.Status < (int)PassengerStatus.Sick)
            {
                crew.MakeWorse();
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
