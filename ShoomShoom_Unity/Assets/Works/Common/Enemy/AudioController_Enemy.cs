using SleepyUtil;
using UnityEngine;
public class AudioController_Enemy : MonoBehaviour
{
    [Header("Main sound")]
    [SerializeField] AudioSource _mainAudioSource;

    [Header("Secondary sound")]
    [SerializeField] AudioSource _secondaryAudioSource;
    [SerializeField] AudioClip _steamRelease;

    [Range(0f, 0.5f)]
    [SerializeField] float _commonVolumeRange = 0.05f;
    [Range(0f, 0.5f)]
    [SerializeField] float _commonPitchRange = 0.2f;

    public void PlaySteamReleaseSound()
    {
        _secondaryAudioSource.SetRandomClip(ref _steamRelease, .1f, _commonVolumeRange, _commonPitchRange);
    }
}