using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ExtrasScreen : VisualElement
{
    private readonly ExtrasButtonGroup _extrasButtonGroup = new();
    private readonly SaveMenu _saveMenu;
    private readonly JournalSelector _journalSelector = new();
    private readonly JournalPanel _journalPanel = new();

    public ExtrasScreen()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.unityFont = Resources.Load<Font>("Fonts/ronix");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/ronix")
        );
        style.backgroundImage = UiUtils.LoadTexture("background", Scene.Extras);
        style.position = Position.Relative;
        style.display = DisplayStyle.Flex;

        // have to initialize here because member method like SwitchToMenuView is not available in field initializer
        _saveMenu = new(
            "Campaign Mode",
            CampaignModeManager.GetSaveFilePath,
            () => SwitchToMenuView(),
            () => SceneManager.LoadScene((int)Scene.CampaignMode),
            () => SceneManager.LoadScene((int)Scene.CampaignMode)
        );
        ApplyFloatingMenuStyle(_saveMenu);
        ApplyFloatingMenuStyle(_journalSelector);
        _journalSelector.style.height = UiUtils.GetLengthPercentage(90);

        Add(_extrasButtonGroup);
    }

    public void SwitchToSaveFileView()
    {
        Clear();
        Add(_extrasButtonGroup);
        Add(_saveMenu);
    }

    public void SwitchToJournalSelectView()
    {
        Clear();
        Add(_extrasButtonGroup);
        Add(_journalSelector);
    }

    public void SwitchToJournalView(int index)
    {
        Clear();
        Add(_journalPanel);
        _journalPanel.SetText($"Journal {index}");
    }

    public void SwitchToMenuView()
    {
        Clear();
        Add(_extrasButtonGroup);
    }

    private void ApplyFloatingMenuStyle(VisualElement visualElement)
    {
        visualElement.style.position = Position.Absolute;
        visualElement.style.top = UiUtils.GetLengthPercentage(50);
        visualElement.style.left = UiUtils.GetLengthPercentage(50);
        visualElement.style.translate = new Translate(UiUtils.GetLengthPercentage(-50), UiUtils.GetLengthPercentage(-50));
    }
}
