using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CityModeMenu : VisualElement
{
    public CityModeMenu()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.justifyContent = Justify.Center;
        style.width = UiUtils.GetLengthPercentage(20);
        style.alignItems = Align.Center;
        style.unityFont = Resources.Load<Font>("Fonts/ronix");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/ronix")
        );

        Button mainMenuButton = new()
        {
            text = "MAIN MENU",
            style = { width = UiUtils.GetLengthPercentage(100) },
        };
        UiUtils.ApplyCommonMenuButtonStyle(mainMenuButton);

        mainMenuButton.clicked += () =>
        {
            SceneManager.LoadScene((int)Scene.MainMenu);
        };

        Button saveGameButton = new()
        {
            text = "SAVE GAME",
            style = { width = UiUtils.GetLengthPercentage(100) },
        };
        UiUtils.ApplyCommonMenuButtonStyle(saveGameButton);

        saveGameButton.clicked += () =>
        {
            UiManager.Instance.Modal.Show(
                new SaveMenu(
                    "SAVE GAME",
                    CityModeManager.GetSaveFilePath,
                    () =>
                        UiManager.Instance.Modal.Show(
                            UiManager.Instance.CityModeScreen.cityModeMenu
                        ),
                    null,
                    null,
                    () =>
                    {
                        bool success = CityModeManager.Instance.SaveGame();

                        if (!success)
                            UiUtils.ShowError(
                                "Something went wrong while trying to save this game."
                            );
                        else
                        {
                            UiUtils.ShowError(
                                $"Game saved to file {PlayerPrefs.GetInt(SaveMenu.PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY) + 1}."
                            );
                            UiManager.Instance.Modal.Show(
                                UiManager.Instance.CityModeScreen.cityModeMenu
                            );
                        }
                    }
                )
            );
        };

        Add(mainMenuButton);
        Add(saveGameButton);

        RegisterCallback<AttachToPanelEvent>(
            (e) =>
            {
                InputManager.Instance.InputAction.CityMode.Disable();
                InputManager.Instance.InputAction.CityModeMenu.Enable();
                InputManager.Instance.InputAction.CityModeMenu.CloseMenu.performed += OnClose;
            }
        );

        RegisterCallback<DetachFromPanelEvent>(
            (e) =>
            {
                InputManager.Instance.InputAction.CityMode.Enable();
                InputManager.Instance.InputAction.CityModeMenu.CloseMenu.performed -= OnClose;
                InputManager.Instance.InputAction.CityModeMenu.Disable();
            }
        );
    }

    private void OnClose(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        UiManager.Instance.Modal.Close();
    }
}
