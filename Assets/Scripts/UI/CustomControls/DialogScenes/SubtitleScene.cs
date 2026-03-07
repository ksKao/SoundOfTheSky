using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SubtitleScene : VisualElement
{
    public readonly Label titleLabel = new()
    {
        style =
        {
            fontSize = 32,
            unityFontStyleAndWeight = FontStyle.Bold,
            color = Color.white,
        },
    };
    public readonly Label textLabel = new()
    {
        style =
        {
            fontSize = 24,
            color = Color.white,
            width = UiUtils.GetLengthPercentage(75),
            whiteSpace = WhiteSpace.Normal,
            unityTextAlign = TextAnchor.MiddleCenter,
        },
    };

    public SubtitleScene()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
        style.position = Position.Relative;
        style.flexDirection = FlexDirection.Column;
        style.backgroundColor = new Color(1, 0, 0, 0);

        Add(titleLabel);
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
