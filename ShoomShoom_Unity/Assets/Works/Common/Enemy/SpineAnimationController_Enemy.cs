using Spine;
using UniRx;
using UnityEngine;

namespace SleepySpine
{
    public class SpineAnimationController_Enemy : SpineAnimationController
    {
        const int MAIN_TRACK = 0;
        const int SECONDARY_TRACK = 1;

        [SerializeField] protected EnemyController _enemyController;

        TrackEntry _trackMain => _skeletonAnimation.GetCurrentEntry(MAIN_TRACK);
        TrackEntry _trackSecondary => _skeletonAnimation.GetCurrentEntry(SECONDARY_TRACK);

        private void Start()
        {
            _enemyController.CurEnemyState.State.Subscribe(state =>
            {
                switch (state)
                {
                    case EnemyState.Idle:
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "idle", true);
                        break;
                    case EnemyState.TurnLeft:
                        break;
                    case EnemyState.TurnRight:
                        break;
                    case EnemyState.Run:
                        _spineAnimationState.SetAnimation(MAIN_TRACK, "run", true);
                        break;
                    case EnemyState.Jump:
                        break;
                    case EnemyState.Fall:
                        break;
                    case EnemyState.Land:
                        break;
                    default:
                        break;
                }
            });
        }
    }
}