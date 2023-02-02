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
                        break;
                    case PlayerState.Fall:
                        break;
                    case PlayerState.Land:
                        break;
                    default:
                        break;
                }
            }).AddTo(this);
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
