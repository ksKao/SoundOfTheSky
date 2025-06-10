using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeMenuButton : VisualElement
{
    public CampaignModeMenuButton()
    {
        style.position = Position.Absolute;
        style.left = 32;
        style.bottom = 16;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.alignItems = Align.FlexStart;
        style.unityFont = Resources.Load<Font>("Fonts/ronix");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/ronix")
        );
        style.width = 192;
        style.height = UiUtils.GetLengthPercentage(60);

        VisualElement menu = new()
        {
            style =
            {
                backgroundColor = Color.black,
                flexGrow = 1,
                width = UiUtils.GetLengthPercentage(100),
                marginBottom = 16,
                opacity = 0.93f,
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.FlexStart,
                paddingTop = 24,
                paddingBottom = 24,
                paddingLeft = 24,
                paddingRight = 24,
            },
        };

        UiUtils.ToggleBorder(menu, true, Color.white);
        UiUtils.SetBorderWidth(menu, 1);

        menu.visible = false;

        Add(menu);

        Button saveButton = new()
        {
            text = "Save",
            style =
            {
                backgroundColor = Color.clear,
                fontSize = 20,
                color = Color.white,
            },
        };

        UiUtils.ToggleBorder(saveButton, false);

        menu.Add(saveButton);

        saveButton.clicked += () =>
        {
            UiManager.Instance.Modal.Show(
                new SaveMenu(
                    "SAVE GAME",
                    CampaignModeManager.GetSaveFilePath,
                    UiManager.Instance.Modal.Close,
                    null,
                    null,
                    () =>
                    {
                        bool success = CampaignModeManager.Instance.SaveGame();

                        if (!success)
                            UiUtils.ShowError(
                                "Something went wrong while trying to save this game."
                            );
                        else
                        {
                            UiUtils.ShowError(
                                $"Game saved to file {PlayerPrefs.GetInt(SaveMenu.PLAYER_PREFS_SAVE_FILE_TO_LOAD_KEY) + 1}."
                            );
                            UiManager.Instance.Modal.Close();
                        }
                    }
                )
            );
        };

        Button exitButton = new()
        {
            text = "Exit",
            style =
            {
                backgroundColor = Color.clear,
                fontSize = 20,
                color = Color.white,
            },
        };

        UiUtils.ToggleBorder(exitButton, false);

        exitButton.clicked += () =>
        {
            SceneManager.LoadScene((int)Scene.MainMenu);
        };

        menu.Add(exitButton);

        VisualElement buttonContainer = new()
        {
            style =
            {
                position = Position.Relative,
                width = 96,
                height = 32,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
            },
        };

        Add(buttonContainer);

        Button menuButton = new()
        {
            text = "Menu",
            style =
            {
                backgroundColor = Color.clear,
                color = Color.white,
                marginTop = 8,
                marginLeft = 0,
                marginBottom = 0,
                marginRight = 0,
                paddingTop = 0,
                paddingBottom = 0,
                paddingLeft = 0,
                paddingRight = 0,
            },
        };

        UiUtils.ToggleBorder(menuButton, false);

        menuButton.clicked += () =>
        {
            menu.visible = !menu.visible;
        };

        buttonContainer.Add(menuButton);

        buttonContainer.Add(
            new()
            {
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    left = 0,
                    height = UiUtils.GetLengthPercentage(100),
                    width = 2,
                    backgroundColor = Color.white,
                },
            }
        );

        buttonContainer.Add(
            new()
            {
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    left = 0,
                    height = 2,
                    width = 48,
                    backgroundColor = Color.white,
                },
            }
        );
    }
}
