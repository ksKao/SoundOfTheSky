using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RhythmGameScene : VisualElement
{
    private readonly Label _textLabel = new()
    {
        style =
        {
            color = Color.white,
            fontSize = 28,
            whiteSpace = WhiteSpace.Normal,
            width = UiUtils.GetLengthPercentage(100),
            unityTextAlign = TextAnchor.UpperLeft,
        },
    };

    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _currentTween =
        null;

    public string Text
    {
        set
        {
            if (_currentTween != null && _currentTween.active)
                _currentTween.Complete();
            _textLabel.text = value;

            float visibleChars = 0;
            _currentTween = DOTween
                .To(
                    () => visibleChars,
                    x =>
                    {
                        visibleChars = x;
                        _textLabel.text = value.Substring(0, (int)Math.Round(visibleChars));
                    },
                    value.Length,
                    value.Length * 0.02f
                )
                .SetEase(Ease.Linear);
        }
    }

    public RhythmGameScene()
    {
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);

        VisualElement leftPanel = new()
        {
            style =
            {
                height = UiUtils.GetLengthPercentage(100),
                width = UiUtils.GetLengthPercentage(60),
                backgroundColor = new Color(0, 0, 0, 0.6f),
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                paddingTop = 46,
                paddingBottom = 46,
                paddingLeft = 46,
                paddingRight = 46,
            },
        };
        leftPanel.Add(_textLabel);

        VisualElement rightPanel = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                height = UiUtils.GetLengthPercentage(100),
                width = UiUtils.GetLengthPercentage(40),
                backgroundColor = new Color(0, 0, 0, 0.5f),
                paddingTop = 46,
                paddingBottom = 46,
                paddingLeft = 46,
                paddingRight = 46,
            },
        };

        Button beginButton = new()
        {
            style =
            {
                backgroundColor = new Color(0, 0, 0, 0),
                paddingTop = 8,
                paddingBottom = 8,
                paddingLeft = 8,
                paddingRight = 8,
                color = Color.white,
                fontSize = 24,
            },
        };
        UiUtils.ToggleBorder(beginButton, true, Color.white);
        UiUtils.SetBorderWidth(beginButton, 1);
        beginButton.Add(new Label("Start Rhythm Game"));
        beginButton.clicked += () =>
        {
            UiManager.Instance.CampaignModeScreen.dialog.ContinueStory();
        };

        rightPanel.Add(beginButton);

        Add(leftPanel);
        Add(rightPanel);

        leftPanel.RegisterCallback<ClickEvent>(
            (_) =>
            {
                // order is important here for short circuiting, because need to check for active before isComplete()
                if (_currentTween == null || !_currentTween.active || _currentTween.IsComplete())
                    return;

                _currentTween.Complete();
            }
        );

        RegisterCallback<AttachToPanelEvent>(
            (_) =>
            {
                Text = _textLabel.text;
            }
        );
    }
}
