using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TutorialOverlay : VisualElement
{
    // tutorial textbox
    private readonly VisualElement _tutorialTextboxContainer = new()
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
            borderTopColor = Color.black,
            borderBottomColor = Color.black,
            borderLeftColor = Color.black,
            borderRightColor = Color.black,
        },
    };
    private readonly Label _tutorialTextboxLabel = new()
    {
        style =
        {
            flexGrow = 1,
            width = UiUtils.GetLengthPercentage(100),
            whiteSpace = WhiteSpace.Normal,
        },
    };
    private readonly Button _tutorialTextboxButton = new() { text = "Next" };
    private Action _onNext = null;

    // focus
    private VisualElement _prevFocusedElement = null;
    private readonly Dictionary<Direction, VisualElement> _highlightOverlay = Enum.GetValues(
            typeof(Direction)
        )
        .Cast<Direction>()
        .ToDictionary(
            direction => direction,
            direction => new VisualElement()
            {
                style =
                {
                    position = Position.Absolute,
                    backgroundColor = direction == Direction.Center ? Color.clear : Color.black,
                    opacity = direction == Direction.Center ? 1f : 0.98f,
                },
            }
        );
    private float _highlightOverlayPadding = 16;

    public TutorialOverlay()
    {
        _tutorialTextboxButton.clicked += () => _onNext?.Invoke();

        _tutorialTextboxContainer.Add(_tutorialTextboxLabel);
        _tutorialTextboxContainer.Add(_tutorialTextboxButton);
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

        UiManager.Instance.ModalParent.style.position = Position.Relative;
        foreach (VisualElement overlayBlock in _highlightOverlay.Values)
        {
            if (overlayBlock.parent != UiManager.Instance.ModalParent)
                UiManager.Instance.ModalParent.Add(overlayBlock);
        }

        _prevFocusedElement.RegisterCallback<GeometryChangedEvent>(GeometryChangedEventCallback);

        if (_tutorialTextboxContainer.parent != UiManager.Instance.ModalParent)
            UiManager.Instance.ModalParent.Add(_tutorialTextboxContainer);

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
            UiManager.Instance.StartCoroutine(OnNext());
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
            if (overlayBlock.parent == UiManager.Instance.ModalParent)
                UiManager.Instance.ModalParent.Remove(overlayBlock);
        }

        if (_tutorialTextboxContainer.parent == UiManager.Instance.ModalParent)
            UiManager.Instance.ModalParent.Remove(_tutorialTextboxContainer);

        _tutorialTextboxLabel.text = "";
        _onNext = null;
    }

    private void GeometryChangedEventCallback(GeometryChangedEvent e)
    {
        Refocus(e.target as VisualElement);
    }

    private void Refocus(VisualElement e)
    {
        float screenWidth = UiManager.Instance.ModalParent.resolvedStyle.width; // technically is not screen width, because its bound as aspect ratio
        float screenHeight = UiManager.Instance.ModalParent.resolvedStyle.height;
        float x = e.worldBound.position.x - UiManager.Instance.ModalParent.worldBound.position.x;
        float y = e.worldBound.position.y - UiManager.Instance.ModalParent.worldBound.position.y;
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
