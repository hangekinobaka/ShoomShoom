using Spine;
using System.Collections;
using UniRx;
using UnityEngine;

namespace SleepySpine
{
    public class SpineAnimationController_Shoom : SpineAnimationController
    {
        [SerializeField] CharacterController2D _characterController;

        TrackEntry _track0 => _skeletonAnimation.GetCurrentEntry(0);
        TrackEntry _track1 => _skeletonAnimation.GetCurrentEntry(1);

        [Header("Blink setting")]
        [SerializeField] bool _enableBlink = true;
        [SerializeField] float _blinkRandomMin = 0.5f;
        [SerializeField] float _blinkRandomMax = 3f;
        Coroutine _blinkCoroutine;

        private void Start()
        {
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
