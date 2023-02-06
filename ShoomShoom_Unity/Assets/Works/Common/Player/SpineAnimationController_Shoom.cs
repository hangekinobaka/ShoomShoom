using Spine;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace SleepySpine
{
    public class SpineAnimationController_Shoom : SpineAnimationController
    {
        const int MAIN_TRACK = 0;
        const int SECONDARY_TRACK = 1;
        const int GEAR_TRACK = 2;
        const int EQUIP_EFFECT_TRACK = 3;


        [SerializeField] CharacterController2D _characterController;
        [SerializeField] EffectController_Shoom _effectController;
        [SerializeField] SpineSkinSwitcher_Shoom _skinSwitcher;

        TrackEntry _trackMain => _skeletonAnimation.GetCurrentEntry(MAIN_TRACK);
        TrackEntry _trackSecondary => _skeletonAnimation.GetCurrentEntry(SECONDARY_TRACK);
        TrackEntry _trackEquip => _skeletonAnimation.GetCurrentEntry(EQUIP_EFFECT_TRACK);

        [Header("Blink setting")]
        [SerializeField] bool _enableBlink = true;
        [SerializeField] float _blinkRandomMin = 0.5f;
        [SerializeField] float _blinkRandomMax = 3f;
        Coroutine _blinkCoroutine;

        //events
        public event UnityAction OnJumpTriggerPulled, OnSteamEjected;

        private void Start()
        {
            // Handle the backpack extras
            if (_skinSwitcher.CurSkin == "normal-with-backpack")
            {
                _spineAnimationState.SetAnimation(GEAR_TRACK, "gear-roll", true);

                // Register effect event handler
                _effectController.OnSteamTankFull += SteamTankFullHandler;
            }

            // Register spine event handler
            _spineAnimationState.Event += AnimEventHandler;

            // Add secondary animations
            if (_enableBlink) StartBlinkCoroutine();

            // Handle different character state
            _characterController.CurPlayerState.State.Subscribe(state =>
            {
                switch (state)
                {
                    case PlayerState.Idle:
                        if (_trackMain?.Animation.ToString() == "land")
                            _spineAnimationState.AddAnimation(MAIN_TRACK, "idle", true, 0);
                        else
                            _spineAnimationState.SetAnimation(MAIN_TRACK, "idle", true);
                        break;
                    case PlayerState.TurnLeft:
                        break;
                    case PlayerState.TurnRight:
                        break;
                    case PlayerState.Run:
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "run", true);
                        break;
                    case PlayerState.Jump:
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "jump", false);
                        break;
                    case PlayerState.DoubleJump:
                        _characterController.Jump();
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "double-jump", false);
                        break;
                    case PlayerState.Fall:
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "jump-fall", false);
                        _spineAnimationState.AddAnimation(MAIN_TRACK, "fall", true, 0);
                        break;
                    case PlayerState.Land:
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "land", false);
                        break;
                    default:
                        break;
                }
            }).AddTo(this);
        }

        private void OnDisable()
        {
            _spineAnimationState.Event -= AnimEventHandler;
            _effectController.OnSteamTankFull -= SteamTankFullHandler;

            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        private void FixedUpdate()
        {
            UpdateTimeScale();
        }

        private void AnimEventHandler(TrackEntry trackEntry, Spine.Event e)
        {
            string eventName = e.ToString();
            if (eventName == "jump-up")
            {
                _characterController.Jump();
            }
            else if (eventName == "landed")
            {
                _characterController.Landed();
            }
            else if (eventName == "jump-trigger-pulled")
            {
                if (OnJumpTriggerPulled != null) OnJumpTriggerPulled.Invoke();
            }
            else if (eventName == "steam-ejected")
            {
                if (OnSteamEjected != null) OnSteamEjected.Invoke();
            }
        }

        private void SteamTankFullHandler()
        {
            _spineAnimationState.SetAnimation(EQUIP_EFFECT_TRACK, "eject-steam", false);
            _spineAnimationState.AddEmptyAnimation(EQUIP_EFFECT_TRACK, .1f, 1f);
        }

        private void UpdateTimeScale()
        {
            if (_trackMain.Animation.ToString() == "run")
            {
                float speedScale = _characterController.CurRunSpeed / _characterController.MaxSpeed;
                _trackMain.TimeScale = speedScale;
                return;
            }

            if (_trackMain.TimeScale != 1)
            {
                _trackMain.TimeScale = 1;
            }
        }

        private void StartBlinkCoroutine()
        {
            if (_blinkCoroutine != null) return;

            _blinkCoroutine = StartCoroutine(BlinkCoroution());
        }
        IEnumerator BlinkCoroution()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_blinkRandomMin, _blinkRandomMax));
                if (_trackSecondary == null)
                {
                    _spineAnimationState.SetAnimation(SECONDARY_TRACK, "blink", false);
                    _spineAnimationState.AddEmptyAnimation(SECONDARY_TRACK, .1f, .1f);
                }
            }
        }

        public void InterruptEquipTrack()
        {
            _spineAnimationState.SetEmptyAnimation(EQUIP_EFFECT_TRACK, .1f);
        }
    }

}
