using Spine;
using UniRx;
using UnityEngine;

namespace SleepySpine
{
    public class SpineAnimationController_Shoom : SpineAnimationController
    {
        [SerializeField] CharacterController2D _characterController;

        TrackEntry _track0;

        private void Start()
        {
            RegisterSpineEvents();

            _characterController.CurPlayerState.State.Subscribe(state =>
            {
                switch (state)
                {
                    case PlayerState.Idle:
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
                    case PlayerState.Fall:
                        _spineAnimationState.SetAnimation(0, "jump-fall", false);
                        _spineAnimationState.AddAnimation(0, "fall", true, 0);
                        break;
                    case PlayerState.Land:
                        _spineAnimationState.AddAnimation(0, "idle", true, 0);
                        break;
                    default:
                        break;
                }
            }).AddTo(this);
        }

        private void RegisterSpineEvents()
        {
            // Jump event(when shoom really prepared to jump)
            _spineAnimationState.Event += (entry, e) =>
            {
                if (e.ToString() == "jump-up")
                {
                    _characterController.Jump();
                }
            };
        }

        private void FixedUpdate()
        {
            _track0 = _skeletonAnimation.GetCurrentEntry(0);
            UpdateTimeScale();
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
    }

}
