using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public interface TwoButtonMenuScreen
{
    string GetSaveFilePath(int index);
    bool OnSave();
    void DisableScreenInput();
    void EnableScreenInput();
}

[UxmlElement]
public partial class TwoButtonMenu : VisualElement
{
    public TwoButtonMenu() =>
        Debug.LogWarning($"Detected calling the default constructor of {nameof(TwoButtonMenu)}");

    public TwoButtonMenu(TwoButtonMenuScreen screen)
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.justifyContent = Justify.Center;
        style.width = UiUtils.GetLengthPercentage(30);
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
                    screen.GetSaveFilePath,
                    () => UiManager.Instance.Modal.Show(this),
                    null,
                    null,
                    () =>
                    {
                        bool success = screen.OnSave();

                        if (!success)
                            UiUtils.ShowError(
                                "Something went wrong while trying to save this game."
                            );
                        else
                        {
                            UiUtils.ShowError(
                                $"Game saved to file {PlayerPrefs.GetInt(SaveMenu.PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY) + 1}."
                            );
                            UiManager.Instance.Modal.Show(this);
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
                screen.DisableScreenInput();
                InputManager.Instance.InputAction.Menu.Enable();
                InputManager.Instance.InputAction.Menu.CloseMenu.performed += OnClose;
            }
        );

        RegisterCallback<DetachFromPanelEvent>(
            (e) =>
            {
                screen.EnableScreenInput();
                InputManager.Instance.InputAction.Menu.CloseMenu.performed -= OnClose;
                InputManager.Instance.InputAction.Menu.Disable();
            }
        );
    }

    private void OnClose(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        UiManager.Instance.Modal.Close();
    }
}
