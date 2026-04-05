using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ComicScene : VisualElement
{
    private string _text = "";
    private string _speaker = "";
    private string _subtext = "";
    private string _voice = "";
    private bool _attached = false;
    private string _panImage = "";
    private float _panDuration = 0;
    private Tweener _currentTextTween = null;
    private Tweener _currentImageTween = null;
    private readonly VisualElement _comicPanelContainer = new()
    {
        style =
        {
            width = UiUtils.GetLengthPercentage(100),
            position = Position.Relative,
            flexGrow = 1,
        },
    };
    private readonly VisualElement _dialogBox = new()
    {
        style =
        {
            height = UiUtils.GetLengthPercentage(33),
            width = UiUtils.GetLengthPercentage(100),
            paddingTop = 16,
            paddingBottom = 16,
            paddingLeft = 16,
            paddingRight = 16,
            color = Color.white,
            backgroundColor = new Color(0.165f, 0.18f, 0.243f),
            fontSize = 24,
        },
    };
    private readonly Label _nameLabel = new()
    {
        style = { unityFontStyleAndWeight = FontStyle.Bold, marginBottom = 8 },
        enableRichText = true,
    };
    private readonly Label _textLabel = new() { style = { whiteSpace = WhiteSpace.Normal } };

    public ComicScene()
    {
        style.width = UiUtils.GetLengthPercentage(100);
        style.height = UiUtils.GetLengthPercentage(100);
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.backgroundColor = Color.blue;

        Add(_comicPanelContainer);
        Add(_dialogBox);

        _dialogBox.Add(_nameLabel);
        _dialogBox.Add(_textLabel);

        // replay the typing animation && pan image on attach because the transition might be done when switching scenes.
        RegisterCallback<AttachToPanelEvent>(_ =>
        {
            SetText(_text, _speaker, _subtext, _voice);
            PanImage(_panImage, _panDuration);
            _attached = true;
        });

        RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            _attached = false;
            AudioManager.Instance.StopVoice();
        });

        RegisterCallback<ClickEvent>(_ =>
        {
            if (
                _currentTextTween == null
                || !_currentTextTween.IsActive()
                || _currentTextTween.IsComplete()
            )
            {
                UiManager.Instance.CampaignModeScreen.dialog.ContinueStory();
            }
            else
            {
                _currentTextTween.Complete();
            }
        });
    }

    public void SetText(string text, string speaker, string subtext, string voice)
    {
        AudioManager.Instance.StopVoice();

        _dialogBox.style.display = DisplayStyle.Flex;
        _text = text;
        _speaker = speaker;
        _subtext = subtext;
        _voice = voice;

        _currentTextTween?.Complete();
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
            _nameLabel.text = $"<u>{speaker}</u>";

            if (!string.IsNullOrEmpty(subtext))
                _nameLabel.text += $": <i>[{subtext}]</i>";

            _textLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
        }

        _currentTextTween = DOVirtual
            .Int(
                0,
                text.Length,
                text.Length * 0.02f,
                (x) =>
                {
                    _textLabel.text = text.Substring(0, x);
                }
            )
            .SetEase(Ease.Linear);
    }

    public void PanImage(string image, float duration)
    {
        _panImage = image;
        _panDuration = duration;

        if (string.IsNullOrEmpty(image))
        {
            return;
        }

        if (
            _currentImageTween is not null
            && _currentImageTween.active
            && !_currentImageTween.IsComplete()
        )
            _currentImageTween?.Kill();

        Sprite sprite = UiUtils.LoadSprite(image, Scene.DialogMode);
        float containerWidth = _comicPanelContainer.resolvedStyle.width;
        if (!float.IsFinite(containerWidth) || containerWidth <= 0)
            containerWidth = Screen.width;
        float aspectRatio = (float)sprite.texture.height / sprite.texture.width;

        _comicPanelContainer.Clear();

        Image imageEl = new()
        {
            sprite = sprite,
            scaleMode = ScaleMode.StretchToFill,
            style =
            {
                position = Position.Absolute,
                left = 0,
                top = 0,
                width = UiUtils.GetLengthPercentage(100),
                height = containerWidth * aspectRatio,
                translate = new Translate(0, 0),
            },
        };
        _comicPanelContainer.Add(imageEl);

        _currentImageTween = DOVirtual
            .Float(
                0,
                -66,
                duration,
                (x) =>
                {
                    imageEl.style.translate = new Translate(0, UiUtils.GetLengthPercentage(x));
                }
            )
            .SetEase(Ease.Linear);
    }
}
