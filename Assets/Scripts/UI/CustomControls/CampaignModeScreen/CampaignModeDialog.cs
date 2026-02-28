using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeDialog : VisualElement
{
    public CampaignModeDialog()
    {
        Add(new Label("Hello Dialog"));

        Button button = new() { text = "Back to gameplay" };

        button.clicked += () =>
        {
            UiManager.Instance.CampaignModeScreen.ChangeToGameplay();
        };

        Add(button);
    }
}
