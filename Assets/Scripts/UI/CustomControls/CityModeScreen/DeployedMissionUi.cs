using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DeployedMissionUi : VisualElement
{
    private const string DEPLOYED_MISSION_BACKGROUND_IMAGE_PREFIX = "deployed_mission_background_";
    private const string DEPLOYED_MISSION_BACKGROUND_BORDER_IMAGE_PREFIX =
        DEPLOYED_MISSION_BACKGROUND_IMAGE_PREFIX + "border_";
    private const string DEPLOYED_MISSION_CHECK_HEALTH_BUTTON_BACKGROUND_IMAGE_PREFIX =
        "check_health_button_";
    private const string DEPLOYED_MISSION_RESOLVE_BUTTON_BACKGROUND_IMAGE_PREFIX =
        "resolve_button_";
    private const int NUMBER_OF_DEPLOYED_MISSION_BACKGROUND_VARIATIONS = 5;

    private static readonly Sprite _arrivalsBadgeImage = UiUtils.LoadSprite("arrivals_badge");

    private readonly VisualElement _arriveOverlay = new();
    private readonly VisualElement _container = new()
    {
        style =
        {
            width = UiUtils.GetLengthPercentage(100),
            height = UiUtils.GetLengthPercentage(100),
            position = Position.Relative,
            display = DisplayStyle.Flex,
        },
    };
    private readonly VisualElement _contentContainer = new()
    {
        style =
        {
            width = UiUtils.GetLengthPercentage(100),
            height = UiUtils.GetLengthPercentage(100),
            paddingTop = UiUtils.GetLengthPercentage(3f),
            paddingBottom = UiUtils.GetLengthPercentage(3f),
            paddingLeft = UiUtils.GetLengthPercentage(3f),
            paddingRight = UiUtils.GetLengthPercentage(3f),
            flexDirection = FlexDirection.Row,
        },
    };

    public Image trainImage = new()
    {
        style = { width = UiUtils.GetLengthPercentage(100), height = 50 },
    };
    public readonly Button checkHealthButton = new()
    {
        text = "Check\nHealth",
        style =
        {
            unityTextAlign = TextAnchor.MiddleCenter,
            fontSize = 12,
            color = Color.white,
            backgroundColor = Color.clear,
        },
    };
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
        int backgroundNumber = Random.GetRandomIntInRange(
            1,
            NUMBER_OF_DEPLOYED_MISSION_BACKGROUND_VARIATIONS
        );

        style.minHeight = UiUtils.GetLengthPercentage(100 / 6f);
        style.maxHeight = UiUtils.GetLengthPercentage(100 / 6f);
        style.color = UiUtils.darkBlueTextColor;

        Add(_container);

        VisualElement backgroundImage = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture(
                    DEPLOYED_MISSION_BACKGROUND_IMAGE_PREFIX + backgroundNumber
                ),
                position = Position.Absolute,
                width = UiUtils.GetLengthPercentage(95),
                height = UiUtils.GetLengthPercentage(65),
                top = UiUtils.GetLengthPercentage(50),
                left = UiUtils.GetLengthPercentage(50),
                translate = new Translate(
                    UiUtils.GetLengthPercentage(-50),
                    UiUtils.GetLengthPercentage(-52)
                ),
            },
        };
        _container.Add(backgroundImage);

        VisualElement borderImage = new()
        {
            style =
            {
                backgroundImage = UiUtils.LoadTexture(
                    DEPLOYED_MISSION_BACKGROUND_BORDER_IMAGE_PREFIX + backgroundNumber
                ),
                position = Position.Absolute,
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(100),
                top = 0,
                left = 0,
            },
        };
        _container.Add(borderImage);

        _container.Add(_contentContainer);

        _arriveOverlay.style.position = Position.Absolute;
        _arriveOverlay.style.top = UiUtils.GetLengthPercentage(-2);
        _arriveOverlay.style.left = UiUtils.GetLengthPercentage(-0.5f);
        _arriveOverlay.style.width = UiUtils.GetLengthPercentage(100);
        _arriveOverlay.style.height = UiUtils.GetLengthPercentage(100);
        _arriveOverlay.style.display = DisplayStyle.None;
        _arriveOverlay.style.justifyContent = Justify.Center;
        _arriveOverlay.style.alignItems = Align.Center;
        Add(_arriveOverlay);

        Image arriveLabel = new()
        {
            sprite = _arrivalsBadgeImage,
            style = { width = 250, height = 50 },
        };
        _arriveOverlay.Add(arriveLabel);

        VisualElement leftContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(15),
                height = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceBetween,
            },
        };

        VisualElement centerContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(70),
                height = UiUtils.GetLengthPercentage(100),
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceBetween,
            },
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
                justifyContent = Justify.Center,
            },
        };

        _contentContainer.Add(leftContainer);
        _contentContainer.Add(centerContainer);
        _contentContainer.Add(rightContainer);

        milesRemainingLabel.text = mission.MilesRemaining + " miles";
        leftContainer.Add(milesRemainingLabel);

        UiUtils.ToggleBorder(checkHealthButton, false);
        checkHealthButton.style.backgroundImage = UiUtils.LoadTexture(
            DEPLOYED_MISSION_CHECK_HEALTH_BUTTON_BACKGROUND_IMAGE_PREFIX + backgroundNumber
        );
        checkHealthButton.clicked += mission.OnCheckHealthButtonClicked;
        leftContainer.Add(checkHealthButton);

        weatherLabel.text = mission.WeatherSO.name;
        leftContainer.Add(weatherLabel);

        materialLabelsContainer.style.display = DisplayStyle.Flex;
        materialLabelsContainer.style.flexDirection = FlexDirection.Row;
        materialLabelsContainer.style.justifyContent = Justify.SpaceEvenly;
        materialLabelsContainer.style.alignItems = Align.Center;
        materialLabelsContainer.style.width = UiUtils.GetLengthPercentage(100);
        centerContainer.Add(materialLabelsContainer);

        centerContainer.Add(trainImage);

        VisualElement routeLabelContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                width = UiUtils.GetLengthPercentage(100),
            },
        };

        Label missionTypeLabel = new(mission.Type.ToString().ToUpper())
        {
            style =
            {
                color = Color.red,
                unityFontStyleAndWeight = FontStyle.Bold,
                marginRight = 16,
            },
        };
        routeLabelContainer.Add(missionTypeLabel);

        routeLabel.text =
            $"{mission.Type.ToString().ToUpper()} {mission.Route.start.locationSO.name} - {mission.Route.end.locationSO.name}";
        routeLabelContainer.Add(routeLabel);

        centerContainer.Add(routeLabelContainer);

        resolveButton.text = "Resolve";
        resolveButton.visible = mission.EventPending;
        resolveButton.clicked += mission.OnResolveButtonClicked;
        resolveButton.style.backgroundImage = UiUtils.LoadTexture(
            DEPLOYED_MISSION_RESOLVE_BUTTON_BACKGROUND_IMAGE_PREFIX + backgroundNumber
        );
        resolveButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        resolveButton.style.color = Color.white;
        resolveButton.style.backgroundColor = Color.clear;
        UiUtils.ToggleBorder(resolveButton, false);
        rightContainer.Add(resolveButton);
    }

    public void Arrive()
    {
        _container.style.opacity = 0.7f;
        _arriveOverlay.style.display = DisplayStyle.Flex;
        _arriveOverlay.BringToFront();
        _arriveOverlay.RegisterCallback<ClickEvent>(
            (_) =>
            {
                _contentContainer.Clear();
                _container.style.opacity = 1;
                _arriveOverlay.style.display = DisplayStyle.None;
                _contentContainer.Add(mission.MissionCompleteUi);
            }
        );
    }
}
