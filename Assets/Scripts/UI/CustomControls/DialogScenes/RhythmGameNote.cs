using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RhythmGameNote : VisualElement
{
    private static readonly Sprite s_heidiCircleSprite = UiUtils.LoadSprite(
        "heidi_circle",
        Scene.DialogMode
    );
    private bool _hit = false;

    public bool Hit
    {
        get => _hit;
        set
        {
            if (value)
            {
                style.opacity = 0.5f;
            }

            _hit = value;
        }
    }
    public BeatmapNote Note { get; private set; }

    public RhythmGameNote()
    {
        Debug.LogWarning($"Calling default constructor of {nameof(RhythmGameNote)}");
    }

    public RhythmGameNote(BeatmapNote note)
    {
        Note = note;

        style.position = Position.Absolute;
        style.bottom = UiUtils.GetLengthPercentage(100);
        style.left = UiUtils.GetLengthPercentage(50);
        style.translate = new Translate(UiUtils.GetLengthPercentage(-50), 0);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;

        Image image = new()
        {
            sprite = s_heidiCircleSprite,
            style = { translate = new Translate(0, UiUtils.GetLengthPercentage(-50)) },
        };

        if (note.endSecond != -1)
        {
            VisualElement bar = new()
            {
                style =
                {
                    width = UiUtils.GetLengthPercentage(100),
                    flexGrow = 1,
                    backgroundColor = new Color(1, 1, 1, 0.3f),
                },
            };
            Add(bar);

            float ratio =
                (note.endSecond - note.startSecond) / (note.startSecond - note.showSecond);
            style.height = UiUtils.GetLengthPercentage(ratio * 90);
        }

        Add(image);
    }
}
