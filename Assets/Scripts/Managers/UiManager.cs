using System;
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
}
