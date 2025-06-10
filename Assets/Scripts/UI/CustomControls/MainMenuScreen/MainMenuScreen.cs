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

        Button campaignButton = new() { text = "Campaign " };
        UiUtils.ApplyCommonMenuButtonStyle(campaignButton);

        Button cityModeButton = new() { text = "City Mode" };
        UiUtils.ApplyCommonMenuButtonStyle(cityModeButton);

        Button settingsButton = new() { text = "Settings" };
        UiUtils.ApplyCommonMenuButtonStyle(settingsButton);

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

        campaignButton.clicked += () =>
        {
            UiManager.Instance.Modal.Show(
                new SaveMenu(
                    "Campaign Mode",
                    CampaignModeManager.GetSaveFilePath,
                    () => UiManager.Instance.Modal.Close(),
                    () => SceneManager.LoadScene((int)Scene.CampaignMode),
                    () => SceneManager.LoadScene((int)Scene.CampaignMode)
                )
            );
        };

        cityModeButton.clicked += () =>
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

        VisualElement buttonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                width = UiUtils.GetLengthPercentage(20),
                marginLeft = 36,
                marginBottom = 36,
            },
        };

        Add(buttonsContainer);

        buttonsContainer.Add(campaignButton);
        buttonsContainer.Add(cityModeButton);
        buttonsContainer.Add(settingsButton);
        buttonsContainer.Add(quitButton);
    }
}
