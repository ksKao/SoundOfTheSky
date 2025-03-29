using UnityEditor.Search;
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
            width = 60
        }
    };
    public readonly Button trainButton = new()
    {
        text = "Train",
        style =
        {
            color = UiUtils.HexToRgb("#74def4"),
            backgroundImage = UiUtils.LoadTexture("bottom_bar_button"),
            width = 60
        }
    };
    public readonly Button deployButton = new()
    {
        text = "Deploy",
        style =
        {
            color = UiUtils.HexToRgb("#74def4"),
            backgroundImage = UiUtils.LoadTexture("bottom_bar_button"),
            width = 60
        }
    };

    public BottomNavigationBar()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.SpaceBetween;

        VisualElement buttonGroup =
            new() { style = { display = DisplayStyle.Flex, flexDirection = FlexDirection.Row } };

        crewButton.clicked += () =>
        {
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.crewList
            );
        };

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
                width = 400
            }
        };
        deployedMissionListButton.clicked += () =>
            UiManager.Instance.GameplayScreen.ChangeRightPanel(
                UiManager.Instance.GameplayScreen.deployedMissionList
            );
        Add(deployedMissionListButton);

        this.Query<Button>().ForEach(button =>
            {
                UiUtils.ToggleBorder(button, false);
                button.style.backgroundColor = Color.clear;
            }
        );
    }
}
