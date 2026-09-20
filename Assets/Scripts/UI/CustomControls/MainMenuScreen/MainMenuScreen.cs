using System.Collections.Generic;
using UnityEngine; // This is needed here for Application.Quit, otherwise the build will fail
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MainMenuScreen : VisualElement
{
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
            SceneManager.LoadScene((int)Scene.StoryMode);
            //UiManager.Instance.Modal.Show(
            //    new SaveMenu(
            //        "Campaign Mode",
            //        CampaignModeManager.GetSaveFilePath,
            //        () => UiManager.Instance.Modal.Close(),
            //        () => SceneManager.LoadScene((int)Scene.CampaignMode),
            //        () => SceneManager.LoadScene((int)Scene.CampaignMode)
            //    )
            //);
        };

        extrasButton.clicked += () =>
        {
            UiManager.Instance.Modal.Show(
                new SaveMenu(
                    "City Mode",
                    CityModeManager.GetSaveFilePath,
                    () => UiManager.Instance.Modal.Close(),
                    () => SceneManager.LoadScene((int)Scene.CityMode),
                    () => SceneManager.LoadScene((int)Scene.CityMode)
                )
            );
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
        UiUtils.ToggleBorder(bottomRightContainer, true, Color.white);
        UiUtils.SetBorderWidth(bottomRightContainer, 1);

        Add(bottomRightContainer);

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
