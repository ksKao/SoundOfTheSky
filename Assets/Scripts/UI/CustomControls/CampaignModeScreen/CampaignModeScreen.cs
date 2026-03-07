using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeScreen : VisualElement
{
    public readonly CampaignModeGameplay gameplay = new();
    public readonly CampaignModeDialog dialog = new();

    public CampaignModeScreen()
    {
        style.minHeight = UiUtils.GetLengthPercentage(100);
        style.unityFont = Resources.Load<Font>("Fonts/myriad_pro");
        style.unityFontDefinition = new StyleFontDefinition(
            Resources.Load<FontAsset>("Fonts/myriad_pro")
        );

        Add(gameplay);
    }

    public void ChangeToGameplay()
    {
        if (Contains(dialog))
            Remove(dialog);

        Add(gameplay);
        gameplay.ShowBottomContainer();
    }

    public void ChangeToDialog(UnityEngine.TextAsset storyJsonAsset)
    {
        if (Contains(gameplay))
            Remove(gameplay);

        Add(dialog);
        dialog.Play(storyJsonAsset);
    }
}
