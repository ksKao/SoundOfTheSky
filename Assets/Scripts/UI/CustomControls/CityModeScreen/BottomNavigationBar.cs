using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class BottomNavigationBar : VisualElement
{
    public readonly Button crewButton = new()
    {
        text = "Crew",
        style =
        {
            color = UiUtils.HexToRgb("#74def4"),
            backgroundImage = UiUtils.LoadTexture("bottom_bar_button"),
            width = 60,
        },
    };
    public readonly Button trainButton = new()
    {
        text = "Train",
        style =
        {
            color = UiUtils.HexToRgb("#74def4"),
            backgroundImage = UiUtils.LoadTexture("bottom_bar_button"),
            width = 60,
        },
    };
    public readonly Button deployButton = new()
    {
        text = "Deploy",
        style =
        {
            color = UiUtils.HexToRgb("#74def4"),
            backgroundImage = UiUtils.LoadTexture("bottom_bar_button"),
            width = 60,
        },
    };

    private readonly Label _deployedMissionListCountLabel = new()
    {
        style =
        {
            marginLeft = 0,
            marginRight = 0,
            marginTop = 0,
            marginBottom = 0,
            paddingLeft = 0,
            paddingRight = 0,
            paddingBottom = 0,
            paddingTop = 0,
        },
    };

    public BottomNavigationBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.SpaceBetween;

        VisualElement buttonGroup = new()
        {
            style = { display = DisplayStyle.Flex, flexDirection = FlexDirection.Row },
        };

        crewButton.clicked += ShowCrewList;

        trainButton.clicked += () =>
            UiManager.Instance.CityModeScreen.trainList.Show(CityModeManager.Instance.Trains);

        deployButton.visible = false;

        buttonGroup.Add(crewButton);
        buttonGroup.Add(trainButton);
        buttonGroup.Add(deployButton);
        Add(buttonGroup);

        VisualElement deployedMissionListButtonContainer = new()
        {
            style = { position = Position.Relative },
        };
        Add(deployedMissionListButtonContainer);

        Button deployedMissionListButton = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture("bottom_bar_deployed_mission_button"),
                width = 400,
            },
        };
        deployedMissionListButton.clicked += () =>
        {
            VisualElement element =
                CityModeManager.Instance.deployedMissions.Count > 0
                    ? UiManager.Instance.CityModeScreen.deployedMissionList
                    : UiManager.Instance.CityModeScreen.map;
            UiManager.Instance.CityModeScreen.ChangeRightPanel(element);
        };
        deployedMissionListButtonContainer.Add(deployedMissionListButton);

        VisualElement deployedMissionListLabelContainer = new()
        {
            style =
            {
                position = Position.Absolute,
                display = DisplayStyle.None,
                backgroundColor = Color.red,
                color = Color.white,
                top = 0,
                left = 0,
                translate = new Translate(
                    UiUtils.GetLengthPercentage(-50),
                    UiUtils.GetLengthPercentage(-50)
                ),
                borderTopLeftRadius = UiUtils.GetLengthPercentage(50),
                borderTopRightRadius = UiUtils.GetLengthPercentage(50),
                borderBottomLeftRadius = UiUtils.GetLengthPercentage(50),
                borderBottomRightRadius = UiUtils.GetLengthPercentage(50),
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                width = 16,
                height = 16,
            },
        };
        deployedMissionListLabelContainer.Add(_deployedMissionListCountLabel);
        deployedMissionListButtonContainer.Add(deployedMissionListLabelContainer);

        this.Query<Button>()
            .ForEach(button =>
            {
                UiUtils.ToggleBorder(button, false);
                button.style.backgroundColor = Color.clear;
            });
    }

    public void ShowCrewList()
    {
        Crew[] selectedCrews = { };

        Button restButton = new() { text = "REST" };
        restButton.clicked += () =>
        {
            VisualElement container = new()
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row,
                    width = UiUtils.GetLengthPercentage(100),
                    justifyContent = Justify.FlexEnd,
                },
            };

            Button applyRestButton = new()
            {
                text = "REST",
                style = { display = DisplayStyle.None },
            };
            applyRestButton.clicked += () =>
            {
                Crew[] selectedCrews = CityModeManager
                    .Instance.crews.Where(c => c.Selected)
                    .ToArray();

                foreach (Crew crew in selectedCrews)
                    crew.isResting = true;

                ShowCrewList();
            };
            container.Add(applyRestButton);

            UiManager.Instance.CityModeScreen.crewSelectionPanel.Show(
                CityModeManager
                    .Instance.crews.Where(c =>
                        !c.isResting
                        && c.deployedMission is null
                        && c.Status != PassengerStatus.Comfortable
                    )
                    .ToArray(),
                (crews) =>
                {
                    int selectedCount = crews.Where(c => c.Selected).Count();

                    if (selectedCount > 0)
                    {
                        applyRestButton.style.display = DisplayStyle.Flex;
                        applyRestButton.text = $"REST\n{selectedCount}";
                    }
                    else
                    {
                        applyRestButton.style.display = DisplayStyle.None;
                        applyRestButton.text = "REST";
                    }
                },
                GetBracketText,
                ShowCrewList,
                container
            );
        };

        Button cureButton = new() { text = "CURE" };
        cureButton.clicked += () =>
        {
            VisualElement container = new()
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row,
                    width = UiUtils.GetLengthPercentage(100),
                    justifyContent = Justify.FlexEnd,
                },
            };

            Button applyCureButton = new()
            {
                text = "CURE",
                style = { display = DisplayStyle.None },
            };
            applyCureButton.clicked += () =>
            {
                Crew[] selectedCrews = CityModeManager
                    .Instance.crews.Where(c => c.Selected)
                    .ToArray();

                if (
                    selectedCrews.Length
                    > CityModeManager.Instance.GetMaterialValue(MaterialType.Supplies)
                )
                {
                    UiUtils.ShowError("Not enough supplies");
                    return;
                }

                CityModeManager.Instance.IncrementMaterialValue(
                    MaterialType.Supplies,
                    -selectedCrews.Count()
                );

                foreach (Crew crew in selectedCrews)
                    crew.MakeBetter();

                ShowCrewList();
            };
            container.Add(applyCureButton);

            UiManager.Instance.CityModeScreen.crewSelectionPanel.Show(
                CityModeManager
                    .Instance.crews.Where(c =>
                        !c.isResting
                        && c.deployedMission is null
                        && c.Status != PassengerStatus.Comfortable
                    )
                    .ToArray(),
                (crews) =>
                {
                    int selectedCount = crews.Where(c => c.Selected).Count();

                    if (selectedCount > 0)
                    {
                        applyCureButton.style.display = DisplayStyle.Flex;
                        applyCureButton.text = $"CURE\n{selectedCount}";
                    }
                    else
                    {
                        applyCureButton.style.display = DisplayStyle.None;
                        applyCureButton.text = "CURE";
                    }
                },
                GetBracketText,
                ShowCrewList,
                container
            );
        };

        Button newCrewButton = new()
        {
            text = "CREW\n$300",
            style = { marginLeft = StyleKeyword.Auto },
        };
        newCrewButton.clicked += () =>
        {
            if (CityModeManager.Instance.GetMaterialValue(MaterialType.Payments) < 300)
            {
                UiUtils.ShowError("Not enough payments");
                return;
            }

            if (CityModeManager.Instance.crews.Count >= CityModeManager.MAX_CREW_COUNT)
            {
                UiUtils.ShowError("You may only have up to 200 crews at a time.");
                return;
            }

            CityModeManager.Instance.IncrementMaterialValue(MaterialType.Payments, -300);
            CityModeManager.Instance.crews.Add(new());

            ShowCrewList();
        };

        VisualElement buttonContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
            },
        };

        buttonContainer.Add(restButton);
        buttonContainer.Add(cureButton);

        if (CityModeManager.Instance.crews.Count < CityModeManager.MAX_CREW_COUNT)
            buttonContainer.Add(newCrewButton);

        UiManager.Instance.CityModeScreen.crewSelectionPanel.Show(
            CityModeManager.Instance.crews.ToArray(),
            (crews) =>
                UiManager.Instance.CityModeScreen.ChangeRightPanel(
                    new CrewUpgradePanel(crews.First(c => c.Selected))
                ),
            GetBracketText,
            null,
            buttonContainer
        );
    }

    public void RefreshEventPendingMissionCount()
    {
        int count = CityModeManager.Instance.deployedMissions.Where(e => e.EventPending).Count();

        _deployedMissionListCountLabel.parent.style.display =
            count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        _deployedMissionListCountLabel.text = count.ToString();
    }

    private string GetBracketText(Crew crew)
    {
        if (crew.isResting)
            return "Resting";
        if (crew.deployedMission is not null)
            return "Deployed";
        return "";
    }
}
