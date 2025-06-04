using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
    public CityModeScreen CityModeScreen { get; private set; }
    public MainMenuScreen MainMenuScreen { get; private set; }

    private VisualElement _modalContent = null;
    private VisualElement _backdrop = null;
    private VisualElement ModalParent =>
        CityModeScreen is not null ? CityModeScreen : MainMenuScreen;
    private VisualElement _prevFocusedElement = null;

    // tutorial textbox
    private VisualElement _tutorialTextboxContainer = null;
    private Label _tutorialTextboxLabel = null;
    private Button _tutorialTextboxButton = null;
    private Action _onNext = null;

    // focus
    private readonly Dictionary<Direction, VisualElement> _highlightOverlay = new();
    private float _highlightOverlayPadding = 16;

    protected override void Awake()
    {
        base.Awake();

        UIDocument uiDocument = FindFirstObjectByType<UIDocument>();

        _backdrop = new()
        {
            style =
            {
                position = Position.Absolute,
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(100),
                backgroundColor = new(new Color(0, 0, 0, 0.4f)),
            },
        };

        if (uiDocument == null)
        {
            Debug.LogWarning($"Could not find {nameof(UIDocument)} object in scene.");
        }
        else
        {
            CityModeScreen = uiDocument.rootVisualElement.Q<CityModeScreen>();
            MainMenuScreen = uiDocument.rootVisualElement.Q<MainMenuScreen>();
        }

        _backdrop.RegisterCallback<ClickEvent>(
            (e) =>
            {
                CloseModal();
            }
        );

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            _highlightOverlay.Add(
                direction,
                new VisualElement()
                {
                    style =
                    {
                        position = Position.Absolute,
                        backgroundColor = direction == Direction.Center ? Color.clear : Color.black,
                        opacity = direction == Direction.Center ? 1f : 0.98f,
                    },
                }
            );
        }

        _tutorialTextboxContainer = new()
        {
            style =
            {
                position = Position.Absolute,
                backgroundColor = Color.white,
                color = Color.black,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.FlexEnd,
                width = UiUtils.GetLengthPercentage(98),
                height = UiUtils.GetLengthPercentage(25),
                marginLeft = UiUtils.GetLengthPercentage(1),
                marginRight = UiUtils.GetLengthPercentage(1),
                paddingLeft = 24,
                paddingRight = 24,
                paddingTop = 24,
                paddingBottom = 24,
                fontSize = 24,
            },
        };

        UiUtils.ToggleBorder(_tutorialTextboxContainer, true);

        _tutorialTextboxLabel = new()
        {
            style =
            {
                flexGrow = 1,
                width = UiUtils.GetLengthPercentage(100),
                whiteSpace = WhiteSpace.Normal,
            },
        };

        _tutorialTextboxButton = new() { text = "Next" };

        _tutorialTextboxButton.clicked += () => _onNext?.Invoke();

        _tutorialTextboxContainer.Add(_tutorialTextboxLabel);
        _tutorialTextboxContainer.Add(_tutorialTextboxButton);
    }

    public void ShowModal(VisualElement content)
    {
        ModalParent.style.position = Position.Relative;

        if (!ModalParent.Children().Contains(_backdrop))
            ModalParent.Add(_backdrop);

        if (ModalParent.Children().Contains(_modalContent)) // if the old content is already there, remove it to be replaced by the new content
            ModalParent.Remove(_modalContent);

        content.style.position = Position.Absolute;
        content.style.translate = new Translate(
            UiUtils.GetLengthPercentage(-50),
            UiUtils.GetLengthPercentage(-50)
        );
        content.style.top = UiUtils.GetLengthPercentage(50);
        content.style.left = UiUtils.GetLengthPercentage(50);
        _modalContent = content;

        ModalParent.Add(content);
    }

    public void ShowConfirmationModal(
        string message,
        Action onConfirm = null,
        Action onCancel = null,
        string confirmText = "Confirm",
        string cancelText = "Cancel"
    )
    {
        VisualElement modal = new()
        {
            style =
            {
                backgroundColor = UiUtils.semiTransparentBlackColor,
                color = Color.white,
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Column,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceEvenly,
                width = UiUtils.GetLengthPercentage(50),
                minHeight = UiUtils.GetLengthPercentage(30),
                paddingLeft = UiUtils.GetLengthPercentage(10),
                paddingRight = UiUtils.GetLengthPercentage(10),
            },
        };

        Label messageLabel = new(message)
        {
            style = { whiteSpace = WhiteSpace.Normal, unityTextAlign = TextAnchor.MiddleCenter },
        };

        modal.Add(messageLabel);

        VisualElement buttonsContainer = new()
        {
            style =
            {
                display = DisplayStyle.Flex,
                flexDirection = FlexDirection.Row,
                width = UiUtils.GetLengthPercentage(60),
                justifyContent = Justify.SpaceBetween,
            },
        };

        Button cancelButton = new()
        {
            text = cancelText,
            style = { flexGrow = 1, marginRight = 16 },
        };

        cancelButton.clicked += () =>
        {
            CloseModal();
            onCancel?.Invoke();
        };

        Button confirmButton = new()
        {
            text = confirmText,
            style = { flexGrow = 1, marginLeft = 16 },
        };

        confirmButton.clicked += () =>
        {
            CloseModal();
            onConfirm?.Invoke();
        };

        buttonsContainer.Add(cancelButton);
        buttonsContainer.Add(confirmButton);

        modal.Add(buttonsContainer);

        ShowModal(modal);
    }

    public void CloseModal()
    {
        ModalParent.Remove(_backdrop);
        ModalParent.Remove(_modalContent);
    }

    public void FocusElement(
        VisualElement element,
        string message,
        Action onNext,
        bool clickable = false,
        int padding = 8
    )
    {
        UnfocusElement(_prevFocusedElement);

        _prevFocusedElement = element;

        ModalParent.style.position = Position.Relative;
        foreach (VisualElement overlayBlock in _highlightOverlay.Values)
        {
            if (overlayBlock.parent != ModalParent)
                ModalParent.Add(overlayBlock);
        }

        _prevFocusedElement.RegisterCallback<GeometryChangedEvent>(GeometryChangedEventCallback);

        if (_tutorialTextboxContainer.parent != ModalParent)
            ModalParent.Add(_tutorialTextboxContainer);

        _tutorialTextboxButton.style.display = clickable ? DisplayStyle.None : DisplayStyle.Flex;

        _tutorialTextboxLabel.text = message;
        _onNext = onNext;
        _highlightOverlayPadding = padding;

        _highlightOverlay[Direction.Center].UnregisterCallback<ClickEvent>(OnElementClick);

        if (clickable)
            _highlightOverlay[Direction.Center].RegisterCallback<ClickEvent>(OnElementClick);

        Button button = new();

        Refocus(_prevFocusedElement);
    }

    private void OnElementClick(ClickEvent e)
    {
        using (var newEvent = new NavigationSubmitEvent() { target = _prevFocusedElement })
        {
            _prevFocusedElement.SendEvent(newEvent);
            StartCoroutine(OnNext());
        }
    }

    private IEnumerator OnNext()
    {
        yield return null; // wait for one frame before calling the on next function in order for the navigation submit event to be triggered first
        _onNext?.Invoke();
    }

    public void UnfocusElement(VisualElement element = null)
    {
        _prevFocusedElement?.UnregisterCallback<GeometryChangedEvent>(GeometryChangedEventCallback);
        _prevFocusedElement = null;

        element?.UnregisterCallback<GeometryChangedEvent>(GeometryChangedEventCallback);

        foreach (VisualElement overlayBlock in _highlightOverlay.Values)
        {
            if (overlayBlock.parent == ModalParent)
                ModalParent.Remove(overlayBlock);
        }

        if (_tutorialTextboxContainer.parent == ModalParent)
            ModalParent.Remove(_tutorialTextboxContainer);

        _tutorialTextboxLabel.text = "";
        _onNext = null;
    }

    private void GeometryChangedEventCallback(GeometryChangedEvent e)
    {
        Refocus(e.target as VisualElement);
    }

    private void Refocus(VisualElement e)
    {
        float screenWidth = ModalParent.resolvedStyle.width; // technically is not screen width, because its bound as aspect ratio
        float screenHeight = ModalParent.resolvedStyle.height;
        float x = e.worldBound.position.x - ModalParent.worldBound.position.x;
        float y = e.worldBound.position.y - ModalParent.worldBound.position.y;
        float width = e.resolvedStyle.width;
        float height = e.resolvedStyle.height;

        _highlightOverlay[Direction.Up].style.top = 0;
        _highlightOverlay[Direction.Up].style.left = 0;
        _highlightOverlay[Direction.Up].style.width = UiUtils.GetLengthPercentage(100);
        _highlightOverlay[Direction.Up].style.height = y - _highlightOverlayPadding;

        _highlightOverlay[Direction.Down].style.top = y + height + _highlightOverlayPadding;
        _highlightOverlay[Direction.Down].style.left = x - _highlightOverlayPadding;
        _highlightOverlay[Direction.Down].style.width = width + _highlightOverlayPadding * 2;
        _highlightOverlay[Direction.Down].style.height =
            screenHeight - y - height - _highlightOverlayPadding;

        _highlightOverlay[Direction.Left].style.top = y - _highlightOverlayPadding;
        _highlightOverlay[Direction.Left].style.left = 0;
        _highlightOverlay[Direction.Left].style.width = x - _highlightOverlayPadding;
        _highlightOverlay[Direction.Left].style.height =
            screenHeight - y + _highlightOverlayPadding;

        _highlightOverlay[Direction.Right].style.top = y - _highlightOverlayPadding;
        _highlightOverlay[Direction.Right].style.left = x + width + _highlightOverlayPadding;
        _highlightOverlay[Direction.Right].style.width =
            screenWidth - x - width - _highlightOverlayPadding;
        _highlightOverlay[Direction.Right].style.height =
            screenHeight - y + _highlightOverlayPadding;

        _highlightOverlay[Direction.Center].style.top = y - _highlightOverlayPadding;
        _highlightOverlay[Direction.Center].style.left = x - _highlightOverlayPadding;
        _highlightOverlay[Direction.Center].style.width = width + _highlightOverlayPadding * 2;
        _highlightOverlay[Direction.Center].style.height = height + _highlightOverlayPadding * 2;

        // check if textbox will cover highlighted element, only need to check y axis since the width is 100% anyways
        // also check if top will be overlapped as well, meaning that if positioning the textbox at either top or bottom will both overlap, then prefer to position it at the bottom
        // this can happen if the highlighted element is very tall
        bool topOverlaps = y < _tutorialTextboxContainer.resolvedStyle.height + 16;
        bool bottomOverlaps = (
            y + height >= screenHeight - _tutorialTextboxContainer.resolvedStyle.height - 16
        );
        if (topOverlaps || (topOverlaps && bottomOverlaps))
            _tutorialTextboxContainer.style.top =
                screenHeight - _tutorialTextboxContainer.resolvedStyle.height - 16;
        else
            _tutorialTextboxContainer.style.top = 16;
    }
}
