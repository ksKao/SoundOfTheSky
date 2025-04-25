using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
    public GameplayScreen GameplayScreen { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        UIDocument uiDocument = FindFirstObjectByType<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogWarning($"Could not find {nameof(UIDocument)} object in scene.");
            GameplayScreen = new GameplayScreen();
        }
        else
        {
            GameplayScreen = uiDocument.rootVisualElement.Q<GameplayScreen>();
        }
    }

}
