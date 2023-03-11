using Spine;
using Spine.Unity;
using System.Collections;
using UniRx;
using UnityEngine;

namespace SleepySpine
{
    public class SpineAnimationController_Enemy : SpineAnimationController
    {
        const int MAIN_TRACK = 0;
        const int SECONDARY_TRACK = 1;
        const int ATTACK_TRACK = 2;
        const int STEAM_TRACK = 3;

        [SerializeField] protected EnemyController _enemyController;
        [SerializeField] SteamEjector _steamEjector;

        TrackEntry _trackMain => _skeletonAnimation.GetCurrentEntry(MAIN_TRACK);
        TrackEntry _trackSecondary => _skeletonAnimation.GetCurrentEntry(SECONDARY_TRACK);


        protected virtual void Start()
        {
            // Register spine event handler
            _spineAnimationState.Event += AnimEventHandler;

            // Register controller events
            _enemyController.OnAttack += Attack;
            _enemyController.OnDistanceAttackStart += AimAndShoot;
            _enemyController.OnDistanceAttackStop += StopAimAndShoot;
            if (_steamEjector != null) _steamEjector.OnSteamEjected += EjectSteam;

            // Get necessary bones
            _aimBone = _skeletonAnimation.Skeleton.FindBone(_aimBoneName);

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

        private void OnDisable()
        {
            _spineAnimationState.Event -= AnimEventHandler;

            _enemyController.OnAttack -= Attack;
            _enemyController.OnDistanceAttackStart -= AimAndShoot;
            _enemyController.OnDistanceAttackStop -= StopAimAndShoot;

            if (_steamEjector != null) _steamEjector.OnSteamEjected -= EjectSteam;
        }

        private void AnimEventHandler(TrackEntry trackEntry, Spine.Event e)
        {
            string eventName = e.ToString();

            if (eventName == "eject")
            {
                _steamEjector.SteamEjected();
            }
        }

        #region Attack
        public void Attack()
        {
        }
        #endregion

        #region aim and shoot(Distance Attack)
        [Header("Aim andd shoot")]
        [SpineBone(dataField: "_skeletonAnimation")]
        [SerializeField] string _aimBoneName;
        Bone _aimBone;

        bool _isAimingAndShooting = false;
        Coroutine AimCoroutine;

        public void AimAndShoot()
        {
            // Play aim anim
            _spineAnimationState.SetAnimation(ATTACK_TRACK, "aim", false);

            // Update
            _isAimingAndShooting = true;
            AimCoroutine = StartCoroutine(UpdateAim());
        }
        public void StopAimAndShoot()
        {
            // Stop Play aim anim
            _spineAnimationState.AddEmptyAnimation(SECONDARY_TRACK, .1f, .1f);

            // Update
            _isAimingAndShooting = false;
            AimCoroutine = null;
        }

        IEnumerator UpdateAim()
        {
            while (_isAimingAndShooting)
            {
                // Aim at the target pos
                Vector3 skeletonSpacePoint = transform.InverseTransformPoint(_enemyController.AttackTargetPos);
                skeletonSpacePoint.x *= _skeletonAnimation.Skeleton.ScaleX;
                skeletonSpacePoint.y *= _skeletonAnimation.Skeleton.ScaleY;
                _aimBone.SetLocalPosition(skeletonSpacePoint);

                yield return null;
            }
        }
        #endregion

        #region Eject
        private void EjectSteam()
        {
            _spineAnimationState.SetAnimation(STEAM_TRACK, "eject-smoke", false);
        }
        #endregion
    }
}