using DG.Tweening;
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
        Add(gameplay);
    }

    public void ChangeToGameplay()
    {
        if (Contains(dialog))
            Remove(dialog);

        Add(gameplay);
        gameplay.ShowBottomContainer();
    }

    public void ChangeToDialog()
    {
        if (Contains(gameplay))
            Remove(gameplay);

        Add(dialog);
    }
}
