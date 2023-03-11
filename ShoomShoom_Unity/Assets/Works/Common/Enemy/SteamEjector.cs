using UnityEngine;
using UnityEngine.Events;

public class SteamEjector : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] bool _fixedIntervalEject = true;
    [ConditionalDisplay("_fixedIntervalEject", true)]
    [SerializeField] float _ejectInterval = 3.0f;
    [ConditionalDisplay("_fixedIntervalEject", true)]
    [SerializeField] bool _randomInterval = true;

    [Header("Eject Effect")]
    [SerializeField] ParticleSystem _smokeEffect;
    [SerializeField] AudioController_Enemy _audio;

    float _curSteamTankVolume = 0f;
    float _curSteamTankVolumeLimit;

    public bool IsEjecting { get; set; }

    //events
    public event UnityAction OnSteamEjected;

    private void Start()
    {
        _curSteamTankVolume = 0f;
        IsEjecting = false;
        CalcTankLimit();
    }

    private void FixedUpdate()
    {
        if (IsEjecting || !_fixedIntervalEject) return;

        // Update tank with time
        _curSteamTankVolume += Time.fixedDeltaTime;
        if (_curSteamTankVolume >= _curSteamTankVolumeLimit)
        {
            // Invoke other effects
            if (OnSteamEjected != null)
                OnSteamEjected.Invoke();

            IsEjecting = true;
        }
    }

    void CalcTankLimit()
    {
        if (_randomInterval)
        {
            _curSteamTankVolumeLimit = Random.Range(_ejectInterval - 1f, _ejectInterval + 1f);
        }
        else
            _curSteamTankVolumeLimit = _ejectInterval;
    }

    public void SteamEjected()
    {
        // Play effect
        _smokeEffect?.Play();
        _audio?.PlaySteamReleaseSound();

        // init
        _curSteamTankVolume = 0f;
        IsEjecting = false;
        CalcTankLimit();
    }
}
