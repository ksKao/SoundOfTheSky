using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeScreen : VisualElement
{
    public readonly CampaignModeGameplay gameplay = new();
    public readonly CampaignModeDialog dialog = new();

    public CampaignModeScreen()
    {
        style.minHeight = UiUtils.GetLengthPercentage(100);
        Add(gameplay);
    }

    public void ChangeToGameplay()
    {
        if (Contains(dialog))
            Remove(dialog);

        Add(gameplay);
        gameplay.ShowBottomContainer();
    }

    public void ChangeToDialog(TextAsset storyJsonAsset)
    {
        if (Contains(gameplay))
            Remove(gameplay);

        Add(dialog);
        dialog.Play(storyJsonAsset);
    }
}
