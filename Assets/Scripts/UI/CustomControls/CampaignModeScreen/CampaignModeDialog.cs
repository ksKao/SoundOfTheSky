using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeDialog : VisualElement
{
    private Story _story;
    private DialogSceneType _dialogSceneType = DialogSceneType.Title;
    private VisualElement _currentSceneElement = null;
    private float _delayDuration = 0;
    private readonly TitleScene _titleScene = new();
    private readonly SubtitleScene _subtitleScene = new();
    private readonly DialogScene _dialogScene = new();
    private readonly RhythmGameScene _rhythmGameScene = new();
    private readonly VisualElement _blankScene = new();
    private readonly Image _backgroundFront = new()
    {
        sprite = UiUtils.LoadSprite("black", Scene.DialogMode),
        style =
        {
            position = Position.Absolute,
            width = UiUtils.GetLengthPercentage(100),
            height = UiUtils.GetLengthPercentage(100),
            top = 0,
            left = 0,
        },
        scaleMode = ScaleMode.StretchToFill,
    };
    private readonly Image _backgroundBack = new()
    {
        sprite = UiUtils.LoadSprite("black", Scene.DialogMode),
        style =
        {
            position = Position.Absolute,
            width = UiUtils.GetLengthPercentage(100),
            height = UiUtils.GetLengthPercentage(100),
            top = 0,
            left = 0,
        },
        scaleMode = ScaleMode.StretchToFill,
    };

    public RhythmGameScene RhythmGameScene => _rhythmGameScene;

    public CampaignModeDialog()
    {
        style.height = UiUtils.GetLengthPercentage(100);
        style.width = UiUtils.GetLengthPercentage(100);
        style.position = Position.Relative;

        Add(_backgroundBack);
        Add(_backgroundFront);
    }

    public void Play(TextAsset storyJsonAsset)
    {
        _story = new(storyJsonAsset.text);

        _story.BindExternalFunction(
            nameof(ChangeScene),
            (string sceneType) =>
            {
                // must handle this change before the delay otherwise the text will not be set in the correct scene
                _dialogSceneType = Enum.Parse<DialogSceneType>(sceneType);
                ExecuteWithDelay(() => ChangeScene(_dialogSceneType));
            }
        );

        _story.BindExternalFunction(
            nameof(FadeBackground),
            (string fileName, float duration) =>
            {
                ExecuteWithDelay(() => FadeBackground(fileName, duration));
            }
        );

        _story.BindExternalFunction(
            nameof(Delay),
            (float duration) =>
            {
                Delay(duration);
            }
        );

        _story.BindExternalFunction(
            nameof(PlayAudio),
            (string fileName) =>
            {
                ExecuteWithDelay(() => PlayAudio(fileName));
            }
        );

        _story.BindExternalFunction(
            nameof(LoopAudio),
            (string fileName) =>
            {
                ExecuteWithDelay(() => LoopAudio(fileName));
            }
        );

        _story.BindExternalFunction(
            nameof(SetAudioVolume),
            (string fileName, float volume) =>
            {
                ExecuteWithDelay(() => SetAudioVolume(fileName, volume));
            }
        );

        _story.BindExternalFunction(
            nameof(FadeAudioVolume),
            (string fileName, float volume, float duration) =>
            {
                ExecuteWithDelay(() => FadeAudioVolume(fileName, volume, duration));
            }
        );

        _story.BindExternalFunction(
            nameof(StopAudio),
            (string fileName) =>
            {
                ExecuteWithDelay(() => StopAudio(fileName));
            }
        );

        _story.BindExternalFunction(
            nameof(SetRhythmGameSong),
            (string songName) =>
            {
                ExecuteWithDelay(() => SetRhythmGameSong(songName));
            }
        );

        ContinueStory();
    }

    public void ContinueStory()
    {
        if (_story == null || !_story.canContinue)
        {
            UiManager.Instance.CampaignModeScreen.ChangeToGameplay();
            return;
        }

        string text = _story.Continue().Replace("\\n", "\n");
        Dictionary<string, string> tags = GetCurrentTags();
        _delayDuration = 0;

        switch (_dialogSceneType)
        {
            case DialogSceneType.Title:
                _titleScene.textLabel.text = text;
                break;
            case DialogSceneType.Subtitle:
                _subtitleScene.textLabel.text = text;
                if (tags.ContainsKey("title"))
                {
                    _subtitleScene.titleLabel.text = tags["title"];
                }
                break;
            case DialogSceneType.Dialog:
                _dialogScene.SetText(
                    text,
                    tags.ContainsKey("speaker")
                        ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tags["speaker"])
                        : ""
                );
                List<string> leftPortraits = new(),
                    centerPortraits = new(),
                    rightPortraits = new();
                if (tags.ContainsKey("left_portraits"))
                {
                    leftPortraits = tags["left_portraits"].Split(",").ToList();
                }
                if (tags.ContainsKey("center_portraits"))
                {
                    centerPortraits = tags["center_portraits"].Split(",").ToList();
                }
                if (tags.ContainsKey("right_portraits"))
                {
                    rightPortraits = tags["right_portraits"].Split(",").ToList();
                }
                _dialogScene.SetPortraits(leftPortraits, centerPortraits, rightPortraits);
                break;
            case DialogSceneType.RhythmGame:
                _rhythmGameScene.Text = text;
                break;
        }
    }

    public void ChangeScene(DialogSceneType sceneType)
    {
        if (_currentSceneElement != null)
            Remove(_currentSceneElement);

        switch (sceneType)
        {
            case DialogSceneType.Title:
                _currentSceneElement = _titleScene;
                break;

            case DialogSceneType.Subtitle:
                _currentSceneElement = _subtitleScene;
                break;

            case DialogSceneType.Blank:
                _currentSceneElement = _blankScene;
                break;

            case DialogSceneType.Dialog:
                _currentSceneElement = _dialogScene;
                break;
            case DialogSceneType.RhythmGame:
                _currentSceneElement = _rhythmGameScene;
                break;
        }

        Add(_currentSceneElement);
    }

    public void FadeBackground(string fileName, float duration)
    {
        Sprite sprite = UiUtils.LoadSprite(fileName, Scene.DialogMode);

        _backgroundBack.sprite = sprite;
        DOTween
            .To(
                () => 1f,
                (x) =>
                {
                    _backgroundFront.style.opacity = x;
                },
                0f,
                duration
            )
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                _backgroundFront.sprite = sprite;
                _backgroundFront.style.opacity = 1;
            });
    }

    public void Delay(float duration)
    {
        _delayDuration += duration;
    }

    public void PlayAudio(string name)
    {
        AudioManager.Instance.PlayAudio(name, false);
    }

    public void LoopAudio(string name)
    {
        AudioManager.Instance.PlayAudio(name, true);
    }

    public void SetAudioVolume(string name, float volume)
    {
        AudioManager.Instance.SetVolume(name, volume);
    }

    public void FadeAudioVolume(string name, float volume, float duration)
    {
        DOTween
            .To(
                () => AudioManager.Instance.GetVolume(name),
                (x) =>
                {
                    AudioManager.Instance.SetVolume(name, x);
                },
                volume,
                duration
            )
            .SetEase(Ease.InCubic);
    }

    public void StopAudio(string name)
    {
        AudioManager.Instance.StopAudio(name);
    }

    public void SetRhythmGameSong(string name)
    {
        _rhythmGameScene.CurrentSong = name;
    }

    private Dictionary<string, string> GetCurrentTags()
    {
        Dictionary<string, string> tags = new();

        foreach (string tag in _story.currentTags)
        {
            string[] tagParts = tag.Split("=");

            if (tagParts.Length == 2)
            {
                string tagKey = tagParts[0].Trim();
                string tagValue = tagParts[1].Trim();

                tags.Add(tagKey, tagValue);
            }
        }

        return tags;
    }

    private void ExecuteWithDelay(Action callback)
    {
        if (_delayDuration == 0f)
        {
            callback();
        }
        else
        {
            DOVirtual.DelayedCall(
                _delayDuration,
                () =>
                {
                    callback();
                }
            );
        }
    }
}
