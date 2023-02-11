using SleepyUtil;
using UnityEngine;
public class AudioController_Shoom : MonoBehaviour
{
    [SerializeField] CharacterController2D _characterController;

    [Header("Main sound")]
    [SerializeField] AudioSource _mainAudioSource;
    [SerializeField] AudioClip[] _footSteps;
    [SerializeField] AudioClip[] _footStepsWater;

    [Header("Secondary sound")]
    [SerializeField] AudioSource _secondaryAudioSource;
    [SerializeField] AudioClip _steamRelease;
    [SerializeField] AudioClip _steamBlast;

    [Header("Pistol sound")]
    [SerializeField] AudioSource _pistolAudioSource;
    [SerializeField] AudioClip _pistolShot;

    [Range(0f, 0.5f)]
    [SerializeField] float _commonVolumeRange = 0.2f;
    [Range(0f, 0.5f)]
    [SerializeField] float _commonPitchRange = 0.2f;

    public void PlayFootStep()
    {
        if (_characterController.CurGroundType == GroundType.Water)
            _mainAudioSource.SetRandomClip(ref _footStepsWater, .8f, _commonVolumeRange, _commonPitchRange);
        else
            _mainAudioSource.SetRandomClip(ref _footSteps, .8f, _commonVolumeRange, _commonPitchRange);
    }
    public void PlayHeavyFootStep()
    {
        if (_characterController.CurGroundType == GroundType.Water)
            _mainAudioSource.SetRandomClipWithFixedVolume(ref _footStepsWater, 1, _commonPitchRange);
        else
            _mainAudioSource.SetRandomClipWithFixedVolume(ref _footSteps, 1, _commonPitchRange);
    }

    public void PlaySteamReleaseSound()
    {
        _secondaryAudioSource.SetRandomClip(ref _steamRelease, .3f, _commonVolumeRange, _commonPitchRange);
    }
    public void PlaySteamBlastSound()
    {
        _secondaryAudioSource.SetRandomClip(ref _steamBlast, .3f, _commonVolumeRange, _commonPitchRange);
    }
    public void PlayPistolShot()
    {
        _pistolAudioSource.SetRandomClip(ref _pistolShot, .05f, .01f, _commonPitchRange);
        PlaySteamBlastSound();
    }
}