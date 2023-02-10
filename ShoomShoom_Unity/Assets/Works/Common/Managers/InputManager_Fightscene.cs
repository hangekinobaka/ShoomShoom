public class InputManager_Fightscene : Singleton<InputManager_Fightscene>
{
    PlayerInputAction _playerInputAction;
    public PlayerInputAction Player => _playerInputAction;

    private void OnEnable()
    {
        _playerInputAction = new PlayerInputAction();
        _playerInputAction.Normal.Enable();
    }
    private void OnDisable()
    {
        _playerInputAction.Normal.Disable();
    }
}
