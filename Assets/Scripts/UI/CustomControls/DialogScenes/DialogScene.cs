using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DialogScene : VisualElement
{
    private string _text = "";
    private string _speaker = "";
    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _currentTween =
        null;
    private readonly Label _nameLabel = new()
    {
        style = { unityFontStyleAndWeight = FontStyle.Bold, marginBottom = 8 },
        enableRichText = true,
    };
    private readonly Label _textLabel = new() { style = { whiteSpace = WhiteSpace.Normal } };

    public DialogScene()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.position = Position.Relative;
        style.display = DisplayStyle.Flex;

        VisualElement textContainer = new()
        {
            style =
            {
                width = UiUtils.GetLengthPercentage(98),
                height = UiUtils.GetLengthPercentage(33),
                position = Position.Absolute,
                bottom = 16,
                left = UiUtils.GetLengthPercentage(50),
                translate = new Translate(UiUtils.GetLengthPercentage(-50), 0),
                backgroundColor = new Color(0.165f, 0.18f, 0.243f, 0.9f),
                fontSize = 24,
                color = Color.white,
                paddingTop = 16,
                paddingBottom = 16,
                paddingLeft = 16,
                paddingRight = 16,
            },
        };

        UiUtils.ToggleBorder(textContainer, true, Color.white);
        UiUtils.SetBorderWidth(textContainer, 1);

        // replay the typing animation on attach because the transition might be done when switching scenes.
        RegisterCallback<AttachToPanelEvent>(_ =>
        {
            SetText(_text, _speaker);
        });

        RegisterCallback<ClickEvent>(_ =>
        {
            if (_currentTween == null || !_currentTween.IsActive() || _currentTween.IsComplete())
            {
                UiManager.Instance.CampaignModeScreen.dialog.ContinueStory();
            }
            else
            {
                _currentTween.Complete();
            }
        });

        Add(textContainer);
        textContainer.Add(_nameLabel);
        textContainer.Add(_textLabel);
    }

    public void SetText(string text, string speaker)
    {
        _text = text;
        _speaker = speaker;
        _currentTween?.Complete();
        _textLabel.text = "";

        if (string.IsNullOrWhiteSpace(speaker))
        {
            _nameLabel.style.display = DisplayStyle.None;
            _textLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
        }
        else
        {
            _nameLabel.style.display = DisplayStyle.Flex;
            _nameLabel.text = $"<u>{speaker}</u>";
            _textLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
        }

        float visibleChars = 0;

        _currentTween = DOTween
            .To(
                () => visibleChars,
                x =>
                {
                    visibleChars = x;
                    _textLabel.text = text.Substring(0, (int)Math.Round(visibleChars));
                },
                text.Length,
                text.Length * 0.02f
            )
            .SetEase(Ease.Linear);
    }
}
