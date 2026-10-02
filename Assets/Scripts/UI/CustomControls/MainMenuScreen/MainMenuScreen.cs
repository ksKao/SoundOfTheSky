using System.Collections.Generic;
using UnityEngine; // This is needed here for Application.Quit, otherwise the build will fail
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MainMenuScreen : VisualElement
{
    private readonly SaveMenu _saveMenu;
    public MainMenuScreen()
    {
        style.position = Position.Relative;
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.justifyContent = Justify.FlexEnd;
        style.unityFont = Resources.Load<Font>("Fonts/ronix");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/ronix")
        );
        style.backgroundImage = UiUtils.LoadTexture("background", Scene.MainMenu);

        Button visualNovelButton = new() { text = "Visual Novel" };
        UiUtils.ApplyCommonMenuButtonStyle(visualNovelButton);

        Button extrasButton = new() { text = "Extras" };
        UiUtils.ApplyCommonMenuButtonStyle(extrasButton);

        Button settingsButton = new() { text = "Settings" };
        UiUtils.ApplyCommonMenuButtonStyle(settingsButton);

        Button followUsButton = new() { text = "Follow Us" };
        UiUtils.ApplyCommonMenuButtonStyle(followUsButton);

        Button quitButton = new() { text = "Quit" };
        UiUtils.ApplyCommonMenuButtonStyle(quitButton);

        _saveMenu = new(
            title: "Visual Novel",
            getSaveFilePath: StoryModeManager.GetSaveFilePath,
            onLoad: () => SceneManager.LoadScene((int)Scene.StoryMode),
            onNew: () => SceneManager.LoadScene((int)Scene.StoryMode),
            onCancel: () => _saveMenu.style.display = DisplayStyle.None
        );
        _saveMenu.style.display = DisplayStyle.None;
        _saveMenu.style.marginBottom = 16;
        _saveMenu.style.backgroundColor = new Color(0, 0, 0, 0.7f);

        quitButton.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };

        visualNovelButton.clicked += () =>
        {
            _saveMenu.style.display = DisplayStyle.Flex;
        };

        extrasButton.clicked += () =>
        {
            SceneManager.LoadScene((int)Scene.Extras);
        };

        VisualElement bottomRightContainer = new()
        {
            style =
            {
                position = Position.Absolute,
                right = 16,
                bottom = 16,
                width = UiUtils.GetLengthPercentage(15),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column
            },
        };

        Add(bottomRightContainer);
        bottomRightContainer.Add(_saveMenu);

        VisualElement buttonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                backgroundColor = new Color(0, 0, 0, 0.7f),
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
            }
        };
        UiUtils.ToggleBorder(buttonsContainer, true, Color.white);
        UiUtils.SetBorderWidth(buttonsContainer, 1);
        bottomRightContainer.Add(buttonsContainer);

        buttonsContainer.Add(CreateButtonGroup(visualNovelButton));
        buttonsContainer.Add(CreateButtonGroup(extrasButton, settingsButton));
        buttonsContainer.Add(CreateButtonGroup(followUsButton, quitButton));

        List<Button> buttons = buttonsContainer.Query<Button>().ToList();

        foreach (Button button in buttons)
        {
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.paddingTop = 8;
            button.style.paddingBottom = 8;
        }
    }

    private VisualElement CreateButtonGroup(params Button[] buttons)
    {
        VisualElement buttonGroup = new()
        {
            style =
            {
                marginTop = 16,
                marginBottom = 16
            }
        };

        foreach (Button button in buttons)
        {
            buttonGroup.Add(button);
        }

        return buttonGroup;
    }
}
