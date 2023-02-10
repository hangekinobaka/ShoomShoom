using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class EffectController_Shoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController2D _characterController;
    [SerializeField] SleepySpine.SpineAnimationController_Shoom _spineAnimationController;
    [SerializeField] ParticleSystem _upSmokeEffect;
    [SerializeField] ParticleSystem _lowSmokeEffect;
    [SerializeField] Transform _gunTopFollower;

    [Header("Parameters")]
    [Tooltip("steam add 1f per second")]
    [SerializeField] float _steamTankVolumeLimit = 5.5f;

    float _curSteamTankVolume = 0f;
    float _curSteamTankVolumeLimit; // I will make the limit random

    bool _isEjecting = false;

    //events
    public event UnityAction OnSteamTankFull;

    private void Start()
    {
        GenRandomTankLimit();
        _curSteamTankVolume = 0f;

        // subscribe animation event
        _spineAnimationController.OnJumpTriggerPulled += PlayBlastJumpSteamEffect;
        _spineAnimationController.OnSteamEjected += PlaySteamEjectEffect;
        _spineAnimationController.OnGunShoot += PlayShootEffect;
    }

    private void OnDisable()
    {
        _spineAnimationController.OnJumpTriggerPulled -= PlayBlastJumpSteamEffect;
        _spineAnimationController.OnSteamEjected -= PlaySteamEjectEffect;
        _spineAnimationController.OnGunShoot -= PlayShootEffect;
    }

    private void FixedUpdate()
    {
        TankStateUpdate();
    }


    void GenRandomTankLimit()
    {
        _curSteamTankVolumeLimit = Random.Range(_steamTankVolumeLimit, _steamTankVolumeLimit + 2);
    }

    private void TankStateUpdate()
    {
        if (_isEjecting) return;
        // Update tank with time
        _curSteamTankVolume += Time.fixedDeltaTime;
        if (_curSteamTankVolume >= _curSteamTankVolumeLimit)
        {
            // Invoke other effects
            if (OnSteamTankFull != null)
                OnSteamTankFull.Invoke();

            _isEjecting = true;
        }
    }

    public void PlaySteamEjectEffect()
    {
        _upSmokeEffect.Play();
        // reset tank
        GenRandomTankLimit();
        _curSteamTankVolume = 0f;
        _isEjecting = false;
    }

    public void PlayBlastJumpSteamEffect()
    {
        // cancel regular tank eject 
        _isEjecting = false;
        _upSmokeEffect.Stop();
        _spineAnimationController.InterruptEquipTrack();

        // reset tank
        _curSteamTankVolume = 0f;

        _lowSmokeEffect.Play();
    }

    public void PlayShootEffect()
    {
        GameObject obj = PoolManager_Fightscene.Instance.GunEffectPool.Get(); ;
        obj.transform.position = _gunTopFollower.position;
        obj.transform.rotation = _gunTopFollower.rotation;

        // Destroy the effect when it fades out
        Observable.Timer(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => PoolManager_Fightscene.Instance.GunEffectPool.Release(obj))
            .AddTo(this);
    }
}
