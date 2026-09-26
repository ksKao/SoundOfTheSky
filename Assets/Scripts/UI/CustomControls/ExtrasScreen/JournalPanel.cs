using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[UxmlElement]
public partial class JournalPanel : VisualElement
{
    private readonly Label _label = new()
    {
        style =
        {
            color = Color.white,
            unityFont = Resources.Load<Font>("Fonts/myriad_pro"),
            unityFontDefinition = new StyleFontDefinition(
                Resources.Load<FontAsset>("Fonts/myriad_pro")),
            whiteSpace = WhiteSpace.Normal
        }
    };

    public JournalPanel()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.position = Position.Relative;
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.FlexStart;
        style.alignItems = Align.FlexEnd;
        style.paddingLeft = 16;
        style.paddingRight = 16;
        style.paddingTop = 20;
        style.paddingBottom = 16;

        ScrollView panel = new(ScrollViewMode.Vertical)
        {
            style =
            {
                position = Position.Relative,
                backgroundColor = UiUtils.HexToRgb("#263c52"),
                width = UiUtils.GetLengthPercentage(90),
                height = UiUtils.GetLengthPercentage(100),
                paddingLeft = 16,
                paddingRight = 16,
                paddingTop = 16,
                paddingBottom = 16,
            }
        };

        Image image = new()
        {
            sprite = UiUtils.LoadSprite("journal_portrait", Scene.Extras),
            style =
            {
                position = Position.Absolute,
                bottom = 0,
                left = UiUtils.GetLengthPercentage(77),
                width = UiUtils.GetLengthPercentage(24),
                height = UiUtils.GetLengthPercentage(75),
            }
        };

        Add(panel);
        Add(image);

        Button closeButton = new()
        {
            style =
            {
                position = Position.Absolute,
                top = 0,
                right = 0,
                backgroundColor = new Color(0, 0, 0, 0),
                backgroundImage = new StyleBackground(UiUtils.LoadVector("x", Scene.Extras)),
            }
        };
        UiUtils.ToggleBorder(closeButton, false);
        closeButton.clicked += () =>
        {
            UiManager.Instance.ExtrasScreen.SwitchToMenuView();
        };

        panel.Add(_label);
        panel.Add(closeButton);
    }

    public void SetText(string text)
    {
        _label.text = text;
    }
}
