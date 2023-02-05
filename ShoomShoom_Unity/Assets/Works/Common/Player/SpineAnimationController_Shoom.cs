using Spine;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace SleepySpine
{
    public class SpineAnimationController_Shoom : SpineAnimationController
    {
        [SerializeField] CharacterController2D _characterController;
        [SerializeField] EffectController_Shoom _effectController;
        [SerializeField] SpineSkinSwitcher_Shoom _skinSwitcher;

        TrackEntry _track0 => _skeletonAnimation.GetCurrentEntry(0);
        TrackEntry _track1 => _skeletonAnimation.GetCurrentEntry(1);

        [Header("Blink setting")]
        [SerializeField] bool _enableBlink = true;
        [SerializeField] float _blinkRandomMin = 0.5f;
        [SerializeField] float _blinkRandomMax = 3f;
        Coroutine _blinkCoroutine;

        //events
        public event UnityAction OnJumpTriggerPulled, OnSteamEjected;

        private void Start()
        {
            // Register spine event handler
            _spineAnimationState.Event += AnimEventHandler;

            // Register effect event handler
            _effectController.OnSteamTankFull += SteamTankFullHandler;

            // Add secondary animations
            if (_enableBlink) StartBlinkCoroutine();
            if (_skinSwitcher.CurSkin == "normal-with-backpack") _spineAnimationState.SetAnimation(2, "gear-roll", true);

            // Handle different character state
            _characterController.CurPlayerState.State.Subscribe(state =>
            {
                switch (state)
                {
                    case PlayerState.Idle:
                        if (_track0?.Animation.ToString() == "land")
                            _spineAnimationState.AddAnimation(0, "idle", true, 0);
                        else
                            _spineAnimationState.SetAnimation(0, "idle", true);
                        break;
                    case PlayerState.TurnLeft:
                        break;
                    case PlayerState.TurnRight:
                        break;
                    case PlayerState.Run:
                        _spineAnimationState.SetAnimation(0, "run", true);
                        break;
                    case PlayerState.Jump:
                        _spineAnimationState.SetAnimation(0, "jump", false);
                        break;
                    case PlayerState.DoubleJump:
                        _characterController.Jump();
                        _spineAnimationState.SetAnimation(0, "double-jump", false);
                        break;
                    case PlayerState.Fall:
                        _spineAnimationState.SetAnimation(0, "jump-fall", false);
                        _spineAnimationState.AddAnimation(0, "fall", true, 0);
                        break;
                    case PlayerState.Land:
                        _spineAnimationState.SetAnimation(0, "land", false);
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
            _spineAnimationState.SetAnimation(3, "eject-steam", false);
        }

        private void UpdateTimeScale()
        {
            if (_track0.Animation.ToString() == "run")
            {
                float speedScale = _characterController.CurRunSpeed / _characterController.MaxSpeed;
                _track0.TimeScale = speedScale;
                return;
            }

            if (_track0.TimeScale != 1)
            {
                _track0.TimeScale = 1;
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
                if (_track1 == null)
                {
                    _spineAnimationState.SetAnimation(1, "blink", false);
                    _spineAnimationState.AddEmptyAnimation(1, .1f, .1f);
                }
            }
        }
    }

}
