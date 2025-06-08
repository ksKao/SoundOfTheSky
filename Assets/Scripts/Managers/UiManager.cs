using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
    public CityModeScreen CityModeScreen { get; private set; }
    public MainMenuScreen MainMenuScreen { get; private set; }
    public CampaignModeScreen CampaignModeScreen { get; private set; }
    public Modal Modal { get; private set; }

    public VisualElement ModalParent =>
        CityModeScreen is not null ? CityModeScreen : MainMenuScreen;

    protected override void Awake()
    {
        base.Awake();

        UIDocument uiDocument = FindFirstObjectByType<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogWarning($"Could not find {nameof(UIDocument)} object in scene.");
        }
        else
        {
            CityModeScreen = uiDocument.rootVisualElement.Q<CityModeScreen>();
            MainMenuScreen = uiDocument.rootVisualElement.Q<MainMenuScreen>();
            CampaignModeScreen = uiDocument.rootVisualElement.Q<CampaignModeScreen>();
        }

        Modal = new();
    }

    public void AddError(string message)
    {
        ModalParent.style.position = Position.Relative;
        float transitionDuration = 2f;

        Label error = new()
        {
            text = message,
            style =
            {
                color = Color.white,
                position = Position.Absolute,
                left = UiUtils.GetLengthPercentage(50),
                top = UiUtils.GetLengthPercentage(15),
                fontSize = 24,
                unityTextOutlineWidth = 2,
                unityTextOutlineColor = Color.black,
                translate = new Translate(UiUtils.GetLengthPercentage(-50), 0),
            },
        };

        ModalParent.Add(error);

        DOTween
            .To(() => 1f, x => error.style.opacity = x, 0f, transitionDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => ModalParent.Remove(error));
        DOTween
            .To(
                () => 15f,
                x => error.style.top = UiUtils.GetLengthPercentage(x),
                14f,
                transitionDuration
            )
            .SetEase(Ease.Linear);
    }
}
