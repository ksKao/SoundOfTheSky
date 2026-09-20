public class InputManager : Singleton<InputManager>
{
    private GameInputAction _inputAction;

    public GameInputAction InputAction => _inputAction;

    protected override void Awake()
    {
        base.Awake();

        _inputAction = new();
        _inputAction.Disable();
    }
}
