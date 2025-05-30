using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
    public CityModeScreen CityModeScreen { get; private set; }
    public MainMenuScreen MainMenuScreen { get; private set; }

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
        }
    }
}
