using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DialogScene : VisualElement
{
    // portrait height as a fraction of the scene height. portraits always use this size, no matter
    // how many of them share a side.
    private const float PortraitHeightRatio = 0.8f;

    // gap kept between the outermost portrait and the edge of the left/right side.
    private const float PortraitEdgeInset = 24;

    // portraits slide over each other when a side runs out of room, but never by more than this
    // much of the narrowest portrait, so nobody gets completely hidden.
    private const float MaxPortraitOverlapRatio = 0.4f;

    private string _text = "";
    private string _speaker = "";
    private string _subtext = "";
    private string _voice = "";
    private bool _attached = false;
    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _currentTween =
        null;
    private readonly Label _nameLabel =
        new() { style = { marginRight = 8 }, enableRichText = true };
    private readonly Label _speakerIconLabel = new() { text = "O)))" };
    private readonly Label _textLabel = new() { style = { whiteSpace = WhiteSpace.Normal } };
    private readonly VisualElement _nameLabelContainer =
        new()
        {
            style =
            {
                marginBottom = 8,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
            },
        };
    private readonly VisualElement _leftPortraitContainer = CreatePortraitContainer();
    private readonly VisualElement _centerPortraitContainer = CreatePortraitContainer();
    private readonly VisualElement _rightPortraitContainer = CreatePortraitContainer();

    public DialogScene()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.position = Position.Relative;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.FlexEnd;

        VisualElement textContainer =
            new()
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
            _attached = true;
            SetText(_text, _speaker, _subtext, _voice);
            AudioManager.Instance.OnVoiceFinish += OnVoiceFinish;
        });

        RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            _attached = false;
            AudioManager.Instance.StopVoice();
            AudioManager.Instance.OnVoiceFinish -= OnVoiceFinish;
        });

        RegisterCallback<ClickEvent>(_ =>
        {
            if (AudioManager.Instance.IsVoicePlaying)
            {
                return;
            }

            if (_currentTween == null || !_currentTween.IsActive() || _currentTween.IsComplete())
            {
                UiManager.Instance.StoryModeScreen.ContinueStory();
            }
            else
            {
                _currentTween.Complete();
            }
        });

        Add(_leftPortraitContainer);
        Add(_centerPortraitContainer);
        Add(_rightPortraitContainer);

        // the portraits are sized from the container, so they have to be laid out again whenever it
        // resizes.
        RegisterPortraitLayout(_leftPortraitContainer, PortraitAnchor.Left);
        RegisterPortraitLayout(_centerPortraitContainer, PortraitAnchor.Center);
        RegisterPortraitLayout(_rightPortraitContainer, PortraitAnchor.Right);

        Add(textContainer);
        textContainer.Add(_nameLabelContainer);
        textContainer.Add(_textLabel);

        _nameLabelContainer.Add(_nameLabel);
        _nameLabelContainer.Add(_speakerIconLabel);
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

        // the portraits usually get set right after this, but the speaker can also change on its own
        // (e.g. when the scene is re-attached), so keep the drawing order in sync here too.
        BringSpeakerToFront(_leftPortraitContainer);
        BringSpeakerToFront(_centerPortraitContainer);
        BringSpeakerToFront(_rightPortraitContainer);

        if (_attached && !string.IsNullOrEmpty(_voice))
        {
            _speakerIconLabel.style.display = DisplayStyle.Flex;
            AudioManager.Instance.PlayVoice(_voice);
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            _nameLabelContainer.style.display = DisplayStyle.None;
            _textLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
        }
        else
        {
            _nameLabelContainer.style.display = DisplayStyle.Flex;
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
        DisplayPortraitsInContainer(leftPortraits, _leftPortraitContainer, PortraitAnchor.Left);
        DisplayPortraitsInContainer(
            centerPortraits,
            _centerPortraitContainer,
            PortraitAnchor.Center
        );
        DisplayPortraitsInContainer(rightPortraits, _rightPortraitContainer, PortraitAnchor.Right);
    }

    private static VisualElement CreatePortraitContainer()
    {
        return new VisualElement
        {
            style =
            {
                height = UiUtils.GetLengthPercentage(100),
                width = UiUtils.GetLengthPercentage(100f / 3),
                display = DisplayStyle.Flex,
                position = Position.Relative,
            },
        };
    }

    private static void RegisterPortraitLayout(VisualElement container, PortraitAnchor anchor)
    {
        container.RegisterCallback<GeometryChangedEvent>(_ => LayoutPortraits(container, anchor));
    }

    private void DisplayPortraitsInContainer(
        List<string> portraits,
        VisualElement container,
        PortraitAnchor anchor
    )
    {
        container.Clear();

        foreach (string portrait in portraits)
        {
            string portraitName = portrait.Trim();

            if (string.IsNullOrEmpty(portraitName))
                continue;

            Sprite sprite = UiUtils.LoadSprite(portraitName, Scene.StoryMode);

            if (sprite == null)
                continue;

            // absolutely positioned so the portraits keep their own size instead of being squeezed
            // by their neighbours, and so the speaker can be moved in front without shifting anyone.
            Image imageElement =
                new()
                {
                    name = portraitName,
                    sprite = sprite,
                    scaleMode = ScaleMode.ScaleToFit,
                    // the drawing order changes with the speaker, so remember the slot this portrait
                    // occupies from left to right.
                    userData = container.childCount,
                    style = { position = Position.Absolute, bottom = 0 },
                };
            container.Add(imageElement);
        }

        LayoutPortraits(container, anchor);
        BringSpeakerToFront(container);
    }

    private static void LayoutPortraits(VisualElement container, PortraitAnchor anchor)
    {
        float containerWidth = container.resolvedStyle.width;
        float containerHeight = container.resolvedStyle.height;

        if (
            container.childCount == 0
            || float.IsNaN(containerWidth)
            || float.IsNaN(containerHeight)
            || containerWidth <= 0
            || containerHeight <= 0
        )
        {
            return;
        }

        // the children are ordered by who is drawn on top, so go back to the left-to-right order.
        List<Image> portraits = container
            .Children()
            .Cast<Image>()
            .OrderBy(portrait => (int)portrait.userData)
            .ToList();

        float portraitHeight = containerHeight * PortraitHeightRatio;
        float[] widths = new float[portraits.Count];
        float totalWidth = 0;
        float narrowestWidth = float.MaxValue;

        for (int i = 0; i < portraits.Count; i++)
        {
            Rect spriteRect = portraits[i].sprite.rect;
            widths[i] = portraitHeight * (spriteRect.width / spriteRect.height);
            totalWidth += widths[i];
            narrowestWidth = Mathf.Min(narrowestWidth, widths[i]);
        }

        float inset = anchor == PortraitAnchor.Center ? 0 : PortraitEdgeInset;
        float availableWidth = containerWidth - inset;
        float overlap = 0;

        // the portraits never resize, so the only way to fit an extra one in is to slide them together.
        if (portraits.Count > 1 && totalWidth > availableWidth)
        {
            overlap = Mathf.Min(
                (totalWidth - availableWidth) / (portraits.Count - 1),
                narrowestWidth * MaxPortraitOverlapRatio
            );
        }

        float rowWidth = totalWidth - (overlap * (portraits.Count - 1));
        float x = anchor switch
        {
            PortraitAnchor.Left => inset,
            PortraitAnchor.Right => containerWidth - inset - rowWidth,
            _ => (containerWidth - rowWidth) / 2,
        };

        for (int i = 0; i < portraits.Count; i++)
        {
            portraits[i].style.width = widths[i];
            portraits[i].style.height = portraitHeight;
            portraits[i].style.left = x;
            x += widths[i] - overlap;
        }
    }

    private void BringSpeakerToFront(VisualElement container)
    {
        if (string.IsNullOrWhiteSpace(_speaker))
            return;

        VisualElement speakerPortrait = null;

        foreach (VisualElement portrait in container.Children())
        {
            // portraits are named after the character they belong to, e.g. "sara_half" is Sara's.
            string character = portrait.name.Split('_')[0];

            if (character.Equals(_speaker.Trim(), StringComparison.OrdinalIgnoreCase))
                speakerPortrait = portrait;
        }

        // the last child is drawn on top, so overlapping portraits never cover the one talking.
        speakerPortrait?.BringToFront();
    }

    private void OnVoiceFinish()
    {
        _speakerIconLabel.style.display = DisplayStyle.None;
    }

    private enum PortraitAnchor
    {
        Left,
        Center,
        Right,
    }
}
