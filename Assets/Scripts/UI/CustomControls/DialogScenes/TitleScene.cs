using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TitleScene : VisualElement
{
    public readonly Label textLabel = new()
    {
        style =
        {
            fontSize = 32,
            unityFontStyleAndWeight = FontStyle.Bold,
            color = Color.white,
        },
    };

    public TitleScene()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
        style.position = Position.Relative;

        Add(textLabel);

        RegisterCallback<ClickEvent>(
            (_) =>
            {
                UiManager.Instance.CampaignModeScreen.dialog.ContinueStory();
            }
        );

        Label continueLabel = new()
        {
            text = "Click anywhere to continue",
            style =
            {
                bottom = 16,
                left = UiUtils.GetLengthPercentage(50),
                translate = new Translate(UiUtils.GetLengthPercentage(-50), 0),
                color = Color.white,
                position = Position.Absolute,
            },
        };

        Add(continueLabel);
    }
}
