using DG.Tweening;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CampaignModeDialog : VisualElement
{
    private Story _story;
    private DialogScene _dialogScene = DialogScene.TitleOnly;
    private readonly TitleScene _titleScene = new();
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

        switch (_dialogScene)
        {
            case DialogScene.TitleOnly:
                _titleScene.textLabel.text = _story.Continue();
                break;
        }
    }

    public void ChangeScene(string sceneType)
    {
        switch (sceneType)
        {
            case "title":
                Clear();
                Add(_titleScene);
                _dialogScene = DialogScene.TitleOnly;
                break;
        }
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
}
