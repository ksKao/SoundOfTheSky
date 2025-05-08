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
        {
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.trainList
            );
        };

        deployButton.visible = false;

        buttonGroup.Add(crewButton);
        buttonGroup.Add(trainButton);
        buttonGroup.Add(deployButton);
        Add(buttonGroup);

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
                GameManager.Instance.deployedMissions.Count > 0
                    ? UiManager.Instance.GameplayScreen.deployedMissionList
                    : UiManager.Instance.GameplayScreen.map;
            UiManager.Instance.GameplayScreen.ChangeRightPanel(element);
        };
        Add(deployedMissionListButton);

        this.Query<Button>()
            .ForEach(button =>
            {
                UiUtils.ToggleBorder(button, false);
                button.style.backgroundColor = Color.clear;
            });
    }

    public void ShowCrewList()
    {
        // UiManager.Instance.GameplayScreen.ChangeRightPanel(
        //     UiManager.Instance.GameplayScreen.crewList
        // );
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
                Crew[] selectedCrews = GameManager.Instance.crews.Where(c => c.Selected).ToArray();

                foreach (Crew crew in selectedCrews)
                    crew.isResting = true;

                ShowCrewList();
            };
            container.Add(applyRestButton);

            UiManager.Instance.GameplayScreen.crewSelectionPanel.Show(
                GameManager
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
                Crew[] selectedCrews = GameManager.Instance.crews.Where(c => c.Selected).ToArray();

                if (
                    selectedCrews.Length
                    > GameManager.Instance.GetMaterialValue(MaterialType.Supplies)
                )
                {
                    UiUtils.ShowError("Not enough supplies");
                    return;
                }

                GameManager.Instance.IncrementMaterialValue(
                    MaterialType.Supplies,
                    -selectedCrews.Count()
                );

                foreach (Crew crew in selectedCrews)
                    crew.MakeBetter();

                ShowCrewList();
            };
            container.Add(applyCureButton);

            UiManager.Instance.GameplayScreen.crewSelectionPanel.Show(
                GameManager
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
            if (GameManager.Instance.GetMaterialValue(MaterialType.Payments) < 300)
            {
                UiUtils.ShowError("Not enough payments");
                return;
            }

            GameManager.Instance.IncrementMaterialValue(MaterialType.Payments, -300);
            GameManager.Instance.crews.Add(new());

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
        buttonContainer.Add(newCrewButton);

        UiManager.Instance.GameplayScreen.crewSelectionPanel.Show(
            GameManager.Instance.crews.ToArray(),
            (crews) =>
                UiManager.Instance.GameplayScreen.ChangeRightPanel(
                    new CrewUpgradePanel(crews.First(c => c.Selected))
                ),
            GetBracketText,
            null,
            buttonContainer
        );
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
