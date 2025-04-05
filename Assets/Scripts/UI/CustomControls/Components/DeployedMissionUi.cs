using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DeployedMissionUi : VisualElement
{
    private const string DEPLOYED_MISSION_BACKGROUND_IMAGE_PREFIX = "deployed_mission_background_";
    private const string DEPLOYED_MISSION_BACKGROUND_BORDER_IMAGE_PREFIX = DEPLOYED_MISSION_BACKGROUND_IMAGE_PREFIX + "border_";
    private const string DEPLOYED_MISSION_CHECK_HEALTH_BUTTON_BACKGROUND_IMAGE_PREFIX = "check_health_button_";
    private const string DEPLOYED_MISSION_RESOLVE_BUTTON_BACKGROUND_IMAGE_PREFIX = "resolve_button_";
    private const int NUMBER_OF_DEPLOYED_MISSION_BACKGROUND_VARIATIONS = 5;

    private readonly VisualElement _arriveOverlay = new();

    public readonly Mission mission;
    public readonly Label milesRemainingLabel = new();
    public readonly Button resolveButton = new();
    public readonly VisualElement materialLabelsContainer = new();
    public readonly Label routeLabel = new();
    public readonly Label weatherLabel = new();

    public DeployedMissionUi()
    {
        Debug.LogWarning($"Detected calling default constructor of {nameof(DeployedMissionUi)}.");
    }

    public DeployedMissionUi(Mission mission)
    {
        this.mission = mission;
        int backgroundNumber = Random.GetRandomIntInRange(1, NUMBER_OF_DEPLOYED_MISSION_BACKGROUND_VARIATIONS);

        style.position = Position.Relative;
        style.paddingTop = UiUtils.GetLengthPercentage(2);
        style.paddingBottom = UiUtils.GetLengthPercentage(2);
        style.paddingLeft = UiUtils.GetLengthPercentage(2);
        style.paddingRight = UiUtils.GetLengthPercentage(2);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.maxHeight = UiUtils.GetLengthPercentage(100 / 6f);
        style.color = UiUtils.HexToRgb("#2c3064");

        VisualElement backgroundImage = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture(DEPLOYED_MISSION_BACKGROUND_IMAGE_PREFIX + backgroundNumber),
                position = Position.Absolute,
                width = UiUtils.GetLengthPercentage(95),
                height = UiUtils.GetLengthPercentage(95),
                top = UiUtils.GetLengthPercentage(50),
                left = UiUtils.GetLengthPercentage(50),
                translate = new Translate(UiUtils.GetLengthPercentage(-50), UiUtils.GetLengthPercentage(-50))
            }
        };
        Add(backgroundImage);

        VisualElement borderImage = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture(DEPLOYED_MISSION_BACKGROUND_BORDER_IMAGE_PREFIX + backgroundNumber),
                position = Position.Absolute,
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(100),
                top = 0,
                left = 0
            }
        };
        Add(borderImage);

        _arriveOverlay.style.position = Position.Absolute;
        _arriveOverlay.style.backgroundColor = new Color(0, 0, 0, 0.5f);
        _arriveOverlay.style.width = UiUtils.GetLengthPercentage(100);
        _arriveOverlay.style.height = UiUtils.GetLengthPercentage(100);
        _arriveOverlay.style.display = DisplayStyle.Flex;
        _arriveOverlay.style.justifyContent = Justify.Center;
        _arriveOverlay.style.alignItems = Align.Center;
        Add(_arriveOverlay);

        Label arriveLabel =
            new()
            {
                style = { fontSize = new StyleLength(48), color = Color.white },
                text = "Arrive!",
            };
        _arriveOverlay.Add(arriveLabel);
        _arriveOverlay.visible = false;

        VisualElement leftContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(15),
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceBetween
            }
        };

        VisualElement centerContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(70),
                height = UiUtils.GetLengthPercentage(100),
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceBetween
            }
        };

        VisualElement rightContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(15),
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.Center
            }
        };

        Add(leftContainer);
        Add(centerContainer);
        Add(rightContainer);

        milesRemainingLabel.text = mission.MilesRemaining + " miles";
        leftContainer.Add(milesRemainingLabel);

        Button checkHealthButton = new()
        {
            text = "Check\nHealth",
            style =
            {
                backgroundImage = UiUtils.LoadTexture(DEPLOYED_MISSION_CHECK_HEALTH_BUTTON_BACKGROUND_IMAGE_PREFIX + backgroundNumber),
                unityTextAlign = TextAnchor.MiddleCenter,
                color = Color.white,
                backgroundColor = Color.clear
            }
        };
        UiUtils.ToggleBorder(checkHealthButton, false);
        leftContainer.Add(checkHealthButton);

        weatherLabel.text = mission.WeatherSO.name;
        leftContainer.Add(weatherLabel);

        materialLabelsContainer.style.display = DisplayStyle.Flex;
        materialLabelsContainer.style.flexDirection = FlexDirection.Row;
        materialLabelsContainer.style.justifyContent = Justify.SpaceEvenly;
        materialLabelsContainer.style.alignItems = Align.Center;
        materialLabelsContainer.style.width = UiUtils.GetLengthPercentage(100);
        centerContainer.Add(materialLabelsContainer);

        if (mission.Train != null)
        {
            centerContainer.Add(new Image()
            {
                sprite = mission.Train.trainSO.sprite,
                style =
                {
                    width = UiUtils.GetLengthPercentage(100)
                }
            });
        }

        routeLabel.text =
            mission.Route.start.locationSO.name + " - " + mission.Route.end.locationSO.name;
        centerContainer.Add(routeLabel);

        resolveButton.text = "Resolve";
        resolveButton.visible = mission.EventPending;
        resolveButton.clicked += mission.OnResolveButtonClicked;
        resolveButton.style.backgroundImage = UiUtils.LoadTexture(DEPLOYED_MISSION_RESOLVE_BUTTON_BACKGROUND_IMAGE_PREFIX + backgroundNumber);
        resolveButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        resolveButton.style.color = Color.white;
        resolveButton.style.backgroundColor = Color.clear;
        UiUtils.ToggleBorder(resolveButton, false);
        rightContainer.Add(resolveButton);
    }

    public void Arrive()
    {
        _arriveOverlay.visible = true;
        _arriveOverlay.BringToFront();
        _arriveOverlay.RegisterCallback<ClickEvent>(
            (_) =>
            {
                Clear();
                Add(mission.MissionCompleteUi);
            }
        );
    }
}
