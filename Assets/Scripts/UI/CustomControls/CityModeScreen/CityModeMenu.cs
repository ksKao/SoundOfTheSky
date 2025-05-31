using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CityModeMenu : VisualElement
{
    public CityModeMenu()
    {
        style.width = 800;
        style.height = 400;
        style.display = DisplayStyle.Flex;
        style.flexDirection = FlexDirection.Column;
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
        style.backgroundColor = Color.gray;

        Add(new Label("Hello World"));

        RegisterCallback<AttachToPanelEvent>(
            (e) =>
            {
                InputManager.Instance.InputAction.CityMode.Disable();
                InputManager.Instance.InputAction.CityModeMenu.Enable();
                InputManager.Instance.InputAction.CityModeMenu.CloseMenu.performed += OnClose;
            }
        );

        RegisterCallback<DetachFromPanelEvent>(
            (e) =>
            {
                InputManager.Instance.InputAction.CityMode.Enable();
                InputManager.Instance.InputAction.CityModeMenu.CloseMenu.performed -= OnClose;
                InputManager.Instance.InputAction.CityModeMenu.Disable();
            }
        );
    }

    private void OnClose(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        UiManager.Instance.CloseModal();
    }
}
