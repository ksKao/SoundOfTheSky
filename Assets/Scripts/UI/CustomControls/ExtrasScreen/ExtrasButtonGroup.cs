using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ExtrasButtonGroup : VisualElement
{
    private readonly Button _bonusButton = CreateButton("Bonus", "#263c52");
    private readonly Button _journalButton = CreateButton("Journal", "#263c52");
    private readonly Button _backButton = CreateButton("Back", "#57636f");

    public ExtrasButtonGroup()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.width = 200;
        style.position = Position.Absolute;
        style.bottom = 16;
        style.left = 16;

        Add(_bonusButton);
        Add(_journalButton);
        Add(_backButton);

        _bonusButton.clicked += () =>
        {
            UiManager.Instance.ExtrasScreen.SwitchToSaveFileView();
        };

        _journalButton.clicked += () =>
        {
            UiManager.Instance.ExtrasScreen.SwitchToJournalSelectView();
        };

        _backButton.clicked += () =>
        {
            SceneManager.LoadScene((int)Scene.MainMenu);
        };
    }

    private static Button CreateButton(string text, string bgColorHex)
    {
        Button button = new()
        {
            text = text,
            style =
            {
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                marginBottom = 8,
                backgroundColor = UiUtils.HexToRgb(bgColorHex),
                color = Color.white,
                fontSize = 12
            }
        };
        UiUtils.SetBorderWidth(button, 1);
        UiUtils.ToggleBorder(button, true, Color.white);

        return button;
    }
}
