using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DialogScene : VisualElement
{
    private string _text = "";
    private string _speaker = "";
    private string _subtext = "";
    private string _voice = "";
    private bool _attached = false;
    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _currentTween =
        null;
    private readonly Label _nameLabel = new()
    {
        style = { marginBottom = 8 },
        enableRichText = true,
    };
    private readonly Label _textLabel = new() { style = { whiteSpace = WhiteSpace.Normal } };
    private readonly VisualElement _leftPortraitContainer = new()
    {
        style =
        {
            height = UiUtils.GetLengthPercentage(100),
            width = UiUtils.GetLengthPercentage(100f / 3),
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            justifyContent = Justify.FlexStart,
            alignItems = Align.FlexEnd,
            paddingLeft = 24,
        },
    };
    private readonly VisualElement _centerPortraitContainer = new()
    {
        style =
        {
            height = UiUtils.GetLengthPercentage(100),
            width = UiUtils.GetLengthPercentage(100f / 3),
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            justifyContent = Justify.Center,
            alignItems = Align.FlexEnd,
        },
    };
    private readonly VisualElement _rightPortraitContainer = new()
    {
        style =
        {
            height = UiUtils.GetLengthPercentage(100),
            width = UiUtils.GetLengthPercentage(100f / 3),
            display = DisplayStyle.Flex,
            flexDirection = FlexDirection.Row,
            justifyContent = Justify.FlexEnd,
            alignItems = Align.FlexEnd,
            paddingRight = 24,
        },
    };

    public DialogScene()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.position = Position.Relative;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.FlexEnd;

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
            SetText(_text, _speaker, _subtext, _voice);
            _attached = true;
        });

        RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            _attached = false;
            AudioManager.Instance.StopVoice();
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

        Add(_leftPortraitContainer);
        Add(_centerPortraitContainer);
        Add(_rightPortraitContainer);

        Add(textContainer);
        textContainer.Add(_nameLabel);
        textContainer.Add(_textLabel);
    }

    public void SetText(string text, string speaker, string subtext, string voice)
    {
        AudioManager.Instance.StopVoice();

        _text = text;
        _speaker = speaker;
        _subtext = subtext;
        _voice = voice;
        _currentTween?.Complete();
        _textLabel.text = "";

        if (_attached && !string.IsNullOrEmpty(_voice))
        {
            AudioManager.Instance.PlayVoice(_voice);
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            _nameLabel.style.display = DisplayStyle.None;
            _textLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
        }
        else
        {
            _nameLabel.style.display = DisplayStyle.Flex;
            _nameLabel.text = $"<b><u>{speaker}</u></b>";

            if (!string.IsNullOrEmpty(subtext))
                _nameLabel.text += $": <i>[{subtext}]</i>";

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

    public void SetPortraits(
        List<string> leftPortraits,
        List<string> centerPortraits,
        List<string> rightPortraits
    )
    {
        DisplayPortraitsInContainer(leftPortraits, _leftPortraitContainer);
        DisplayPortraitsInContainer(centerPortraits, _centerPortraitContainer);
        DisplayPortraitsInContainer(rightPortraits, _rightPortraitContainer);
    }

    private void DisplayPortraitsInContainer(List<string> portraits, VisualElement container)
    {
        container.Clear();

        foreach (string portrait in portraits)
        {
            Image imageElement = new()
            {
                sprite = UiUtils.LoadSprite(portrait, Scene.DialogMode),
                style = { height = UiUtils.GetLengthPercentage(80) },
            };
            container.Add(imageElement);
        }
    }
}
