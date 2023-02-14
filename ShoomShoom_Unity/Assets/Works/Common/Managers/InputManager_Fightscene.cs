using UniRx;
using UnityEngine.InputSystem;

public class InputManager_Fightscene : Singleton<InputManager_Fightscene>
{
    PlayerInputAction _playerInputAction;
    public PlayerInputAction Player => _playerInputAction;

    // Global vals
    public ReactProps<bool> EnableInput = new ReactProps<bool>(false);

    private void Start()
    {
        EnableInput.SetState(false);
        _playerInputAction = new PlayerInputAction();

        SwitchScheme();

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

    void SwitchScheme()
    {

        // Switch scheme for different platform
        if (GameManager.Instance.CurPlatformType == PlatformType.Mobile)
            _playerInputAction.bindingMask = InputBinding.MaskByGroup(_playerInputAction.MobileScheme.bindingGroup);
        else
            _playerInputAction.bindingMask = InputBinding.MaskByGroup(_playerInputAction.PCScheme.bindingGroup);
    }
}
