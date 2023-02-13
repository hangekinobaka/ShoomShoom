using UniRx;

public class InputManager_Fightscene : Singleton<InputManager_Fightscene>
{
    PlayerInputAction _playerInputAction;
    public PlayerInputAction Player => _playerInputAction;

    // Global vals
    public ReactProps<bool> EnableInput = new ReactProps<bool>(false);

    private void Start()
    {
        InputManager_Fightscene.Instance.EnableInput.SetState(false);
        _playerInputAction = new PlayerInputAction();
        EnableInput.State.Subscribe(enabled =>
        {
            SwitchInputState(enabled);
        }).AddTo(this);
    }

    private void OnDisable()
    {
        _playerInputAction.Normal.Disable();
    }

    void SwitchInputState(bool isEnabled)
    {
        if (isEnabled)
        {
            _playerInputAction.Normal.Enable();
        }
        else
        {
            _playerInputAction.Normal.Disable();
        }

    }
}
