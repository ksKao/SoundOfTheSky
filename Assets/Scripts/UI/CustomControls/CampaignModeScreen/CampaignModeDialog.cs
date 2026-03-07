using System.Collections.Generic;
using DG.Tweening;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeDialog : VisualElement
{
    private Story _story;
    private DialogScene _dialogScene = DialogScene.Title;
    private VisualElement _currentSceneElement = null;
    private readonly TitleScene _titleScene = new();
    private readonly SubtitleScene _subtitleScene = new();
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
                ChangeScene(sceneType);
            }
        );

        _story.BindExternalFunction(
            nameof(FadeBackground),
            (string fileName, float duration) =>
            {
                FadeBackground(fileName, duration);
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

        string text = _story.Continue();
        Dictionary<string, string> tags = GetCurrentTags();

        switch (_dialogScene)
        {
            case DialogScene.Title:
                _titleScene.textLabel.text = text;
                break;
            case DialogScene.Subtitle:
                _subtitleScene.textLabel.text = text;
                if (tags.ContainsKey("title"))
                {
                    _subtitleScene.titleLabel.text = tags["title"];
                }
                break;
        }
    }

    public void ChangeScene(string sceneType)
    {
        if (_currentSceneElement != null)
            Remove(_currentSceneElement);

        switch (sceneType)
        {
            case "title":
                _currentSceneElement = _titleScene;
                _dialogScene = DialogScene.Title;
                break;

            case "subtitle":
                _currentSceneElement = _subtitleScene;
                _dialogScene = DialogScene.Subtitle;
                break;

            case "blank":
                _currentSceneElement = _blankScene;
                _dialogScene = DialogScene.Blank;
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

    public Dictionary<string, string> GetCurrentTags()
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
}
