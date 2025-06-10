using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Modal : VisualElement
{
    private VisualElement _modalContent = null;
    private readonly VisualElement _backdrop = null;

    public Modal()
    {
        _backdrop = new()
        {
            style =
            {
                position = Position.Absolute,
                width = UiUtils.GetLengthPercentage(100),
                height = UiUtils.GetLengthPercentage(100),
                backgroundColor = new(new Color(0, 0, 0, 0.8f)),
            },
        };

        _backdrop.RegisterCallback<ClickEvent>(
            (e) =>
            {
                Close();
            }
        );
    }

    public void Show(VisualElement content)
    {
        UiManager.Instance.ModalParent.style.position = Position.Relative;

        if (!UiManager.Instance.ModalParent.Children().Contains(_backdrop))
            UiManager.Instance.ModalParent.Add(_backdrop);

        if (UiManager.Instance.ModalParent.Children().Contains(_modalContent)) // if the old content is already there, remove it to be replaced by the new content
            UiManager.Instance.ModalParent.Remove(_modalContent);

        content.style.position = Position.Absolute;
        content.style.translate = new Translate(
            UiUtils.GetLengthPercentage(-50),
            UiUtils.GetLengthPercentage(-50)
        );
        content.style.top = UiUtils.GetLengthPercentage(50);
        content.style.left = UiUtils.GetLengthPercentage(50);
        _modalContent = content;

        UiManager.Instance.ModalParent.Add(content);
    }

    public void ShowConfirmation(
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
                width = UiUtils.GetLengthPercentage(40),
                minHeight = UiUtils.GetLengthPercentage(20),
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
                width = UiUtils.GetLengthPercentage(100),
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
            Close();
            onCancel?.Invoke();
        };

        Button confirmButton = new()
        {
            text = confirmText,
            style = { flexGrow = 1, marginLeft = 16 },
        };

        confirmButton.clicked += () =>
        {
            Close();
            onConfirm?.Invoke();
        };

        buttonsContainer.Add(cancelButton);
        buttonsContainer.Add(confirmButton);

        modal.Add(buttonsContainer);

        Show(modal);
    }

    public void Close()
    {
        UiManager.Instance.ModalParent.Remove(_backdrop);
        UiManager.Instance.ModalParent.Remove(_modalContent);
    }
}
