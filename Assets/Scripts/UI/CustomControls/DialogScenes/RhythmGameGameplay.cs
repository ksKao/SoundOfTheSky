using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RhythmGameGameplay : VisualElement
{
    private int _numberOfNotes = 0;
    private int _numberOfHitNotes = 0;
    private readonly Label _scoreLabel = new()
    {
        text = "0/0",
        style =
        {
            position = Position.Absolute,
            top = 0,
            right = 0,
            fontSize = 32,
            color = Color.white,
        },
    };
    private static readonly string[] s_images = { "a", "s", "d", "f" };
    private readonly VisualElement _topContainer = new()
    {
        style =
        {
            flexGrow = 1,
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
        },
    };
    private readonly List<VisualElement> _lanes = new(s_images.Length);

    public int NumberOfHitNotes
    {
        get => _numberOfHitNotes;
        set
        {
            _numberOfHitNotes = value;
            _scoreLabel.text = $"{value}/{_numberOfNotes}";
        }
    }
    public int NumberOfNotes
    {
        get => _numberOfNotes;
        set
        {
            _numberOfNotes = value;
            _scoreLabel.text = $"{_numberOfHitNotes}/{value}";
        }
    }

    public RhythmGameGameplay()
    {
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.paddingBottom = 46;
        style.paddingLeft = 46;
        style.paddingRight = 46;
        style.position = Position.Relative;

        Add(_scoreLabel);

        VisualElement bottomContainer = new()
        {
            style =
            {
                height = UiUtils.GetLengthPercentage(10),
                width = UiUtils.GetLengthPercentage(100),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
            },
        };

        for (int i = 0; i < s_images.Length; i++)
        {
            _lanes.Add(
                new()
                {
                    style =
                    {
                        height = UiUtils.GetLengthPercentage(100),
                        width = UiUtils.GetLengthPercentage(100 / s_images.Length),
                        position = Position.Relative,
                    },
                }
            );

            _lanes[i]
                .Add(
                    new Image()
                    {
                        sprite = UiUtils.LoadSprite("alaska_flag", Scene.DialogMode),
                        style =
                        {
                            height = UiUtils.GetLengthPercentage(10),
                            width = UiUtils.GetLengthPercentage(100),
                            position = Position.Absolute,
                            bottom = 0,
                            left = 0,
                        },
                    }
                );
        }

        foreach (string image in s_images)
        {
            Image imageEl = new()
            {
                sprite = UiUtils.LoadSprite(image, Scene.DialogMode),
                style =
                {
                    height = UiUtils.GetLengthPercentage(100),
                    width = UiUtils.GetLengthPercentage(100 / s_images.Length),
                },
            };
            bottomContainer.Add(imageEl);
        }

        Add(_topContainer);
        Add(bottomContainer);

        RegisterCallback<AttachToPanelEvent>(
            (_) => InputManager.Instance.InputAction.RhythmGame.Enable()
        );

        RegisterCallback<DetachFromPanelEvent>(
            (_) => InputManager.Instance.InputAction.RhythmGame.Disable()
        );
    }

    public void Play(string song)
    {
        TextAsset beatmapJsonAsset = Resources.Load<TextAsset>($"Beatmaps/{song}");

        if (beatmapJsonAsset == null || string.IsNullOrWhiteSpace(beatmapJsonAsset.text))
        {
            UiManager.Instance.CampaignModeScreen.dialog.RhythmGameScene.OnRhythmGameFinish();
            return;
        }

        BeatmapNote[] notes = { };

        try
        {
            notes = JsonUtility.FromJson<Beatmap>(beatmapJsonAsset.text).notes;
        }
        catch
        {
            UiManager.Instance.CampaignModeScreen.dialog.RhythmGameScene.OnRhythmGameFinish();
            return;
        }

        if (notes.Length == 0)
        {
            UiManager.Instance.CampaignModeScreen.dialog.RhythmGameScene.OnRhythmGameFinish();
            return;
        }

        AudioManager.Instance.PlayAudio(song, false);
        NumberOfNotes = notes.Length;
        NumberOfHitNotes = 0;

        _topContainer.Clear();

        foreach (VisualElement lane in _lanes)
        {
            _topContainer.Add(lane);
        }

        foreach (BeatmapNote note in notes)
        {
            DOVirtual.DelayedCall(
                note.startSecond,
                () =>
                {
                    RhythmGameNote noteEl = new(note);
                    _lanes[(int)note.lane].Add(noteEl);
                    float end = -15;
                    float duration = note.startSecond - note.showSecond;

                    if (note.endSecond != -1)
                    {
                        duration = note.endSecond - note.showSecond;
                        end = -noteEl.style.height.value.value;
                    }

                    DOVirtual
                        .Float(
                            100,
                            end,
                            duration,
                            (val) =>
                            {
                                noteEl.style.bottom = UiUtils.GetLengthPercentage(val);
                            }
                        )
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            if (!noteEl.Hit)
                                _lanes[(int)note.lane].Remove(noteEl);
                        });
                }
            );
        }

        DOVirtual.DelayedCall(
            AudioManager.Instance.GetSongDuration(song) + 2,
            () =>
            {
                UiManager.Instance.CampaignModeScreen.dialog.RhythmGameScene.OnRhythmGameFinish();
            }
        );
    }

    public void PressLane(RhythmGameLane lane)
    {
        RhythmGameNote noteEl = _lanes[(int)lane]
            .Children()
            .OfType<RhythmGameNote>()
            .OrderBy(note => note.style.bottom)
            .FirstOrDefault();

        // this means that there are no notes in this lane, can just ignore the press
        if (noteEl is null)
        {
            return;
        }

        noteEl.Hit = noteEl.style.bottom.value.value < 5 && noteEl.style.bottom.value.value > -15;

        // only increase hit count when it's a press note, otherwise need to check for hit in release
        if (noteEl.Note.endSecond == -1)
        {
            _lanes[(int)lane].Remove(noteEl);

            if (noteEl.Hit)
            {
                NumberOfHitNotes++;
            }
        }
    }

    public void ReleaseLane(RhythmGameLane lane)
    {
        RhythmGameNote noteEl = _lanes[(int)lane]
            .Children()
            .OfType<RhythmGameNote>()
            .OrderBy(note => note.style.bottom)
            .FirstOrDefault();

        // check to make sure that it's a hold note first before checking if it's hit or not
        // the behavior is that late start hold on drag note will not count, thus ignore and let it disappear by itself
        if (noteEl is null || noteEl.Note.endSecond == -1 || !noteEl.Hit)
        {
            return;
        }

        // bottom should be negative value here, so it's actually a minus operation
        bool hit = (noteEl.style.height.value.value + noteEl.style.bottom.value.value) < 10;

        if (hit)
        {
            NumberOfHitNotes++;
            _lanes[(int)lane].Remove(noteEl);
        }
    }
}
