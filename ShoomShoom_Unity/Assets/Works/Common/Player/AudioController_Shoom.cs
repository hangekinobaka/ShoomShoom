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

    [Range(0f, 0.5f)]
    [SerializeField] float volumeRange = 0.2f;
    [Range(0f, 0.5f)]
    [SerializeField] float pitchRange = 0.2f;

    public void PlayFootStep()
    {
        if (_characterController.CurGroundType == GroundType.Water)
            _mainAudioSource.SetRandomClip(ref _footStepsWater, .8f, volumeRange, pitchRange);
        else
            _mainAudioSource.SetRandomClip(ref _footSteps, .8f, volumeRange, pitchRange);
    }
    public void PlayHeavyFootStep()
    {
        if (_characterController.CurGroundType == GroundType.Water)
            _mainAudioSource.SetRandomClipWithFixedVolume(ref _footStepsWater, 1, pitchRange);
        else
            _mainAudioSource.SetRandomClipWithFixedVolume(ref _footSteps, 1, pitchRange);
    }

    public void PlaySteamReleaseSound()
    {
        _secondaryAudioSource.SetRandomClip(ref _steamRelease, .3f, volumeRange, pitchRange);
    }
    public void PlaySteamBlastSound()
    {
        _secondaryAudioSource.SetRandomClip(ref _steamBlast, .3f, volumeRange, pitchRange);
    }
}