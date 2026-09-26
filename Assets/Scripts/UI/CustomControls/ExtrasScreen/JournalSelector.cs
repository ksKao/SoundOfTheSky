using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class JournalSelector : VisualElement
{
    private readonly Image _backgroundImage = new()
    {
        sprite = UiUtils.LoadSprite("journal_menu", Scene.Extras),
        pickingMode = PickingMode.Ignore,
        style =
        {
            position = Position.Absolute,
            height = UiUtils.GetLengthPercentage(100),
            width = StyleKeyword.Auto,
            top = UiUtils.GetLengthPercentage(50),
            left = UiUtils.GetLengthPercentage(50),
            translate = new Translate(UiUtils.GetLengthPercentage(-50), UiUtils.GetLengthPercentage(-50)),
        },
    };

    public JournalSelector()
    {
        pickingMode = PickingMode.Ignore;
        style.position = Position.Relative;
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);

        Add(_backgroundImage);

        (float top, float left, float height, float width)[] buttonDimensions =
        {
            (6, 25f, 17.5f, 25),
            (33, 57, 13, 14f),
            (43.5f, 33.5f, 9.5f, 14.5f),
            (51, 57, 9.5f, 19),
            (65, 55, 31, 15f),
            (80.5f, 23.5f, 18, 14)
        };

        for (int i = 0; i < buttonDimensions.Length; i++)
        {
            (float top, float left, float height, float width) = buttonDimensions[i];
            Add(CreateButton(top, left, height, width, i));
        }
    }

    private Button CreateButton(float top, float left, float height, float width, int index)
    {
        Button button = new()
        {
            style =
            {
                position = Position.Absolute,
                top = UiUtils.GetLengthPercentage(top),
                left = UiUtils.GetLengthPercentage(left),
                width = UiUtils.GetLengthPercentage(width),
                height = UiUtils.GetLengthPercentage(height),
                backgroundColor = new Color(0, 0, 0, 0)
            }
        };
        UiUtils.ToggleBorder(button, false);

        button.clicked += () =>
        {
            UiManager.Instance.ExtrasScreen.SwitchToJournalView(index);
        };

        return button;
    }
}
