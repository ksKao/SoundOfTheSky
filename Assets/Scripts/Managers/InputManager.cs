public class InputManager : Singleton<InputManager>
{
    private GameInputAction _inputAction;

    public GameInputAction InputAction => _inputAction;

    protected override void Awake()
    {
        base.Awake();

        _inputAction = new();
    }

    protected void OnEnable()
    {
        _inputAction.Main.Enable();
    }

    protected void OnDisable()
    {
        _inputAction.Main.Disable();
    }
}
