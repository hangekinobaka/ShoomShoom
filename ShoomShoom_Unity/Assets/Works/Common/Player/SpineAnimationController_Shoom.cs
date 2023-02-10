using Spine;
using Spine.Unity;
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
        const int WEAPON_TRACK = 4;
        const int WEAPON_SECONDARY_TRACK = 5;

        [SerializeField] CharacterController2D _characterController;
        [SerializeField] EffectController_Shoom _effectController;
        [SerializeField] SpineSkinSwitcher_Shoom _skinSwitcher;
        [SerializeField] AudioController_Shoom _audioController;

        TrackEntry _trackMain => _skeletonAnimation.GetCurrentEntry(MAIN_TRACK);
        TrackEntry _trackSecondary => _skeletonAnimation.GetCurrentEntry(SECONDARY_TRACK);

        [Header("Blink setting")]
        [SerializeField] bool _enableBlink = true;
        [SerializeField] float _blinkRandomMin = 0.5f;
        [SerializeField] float _blinkRandomMax = 3f;
        Coroutine _blinkCoroutine;

        //events
        public event UnityAction OnJumpTriggerPulled, OnSteamEjected, OnGunShoot;

        #region aim and shoot
        [Header("Aim andd shoot")]
        [SpineBone(dataField: "_skeletonAnimation")]
        [SerializeField] string _aimBoneName;
        Bone _aimBone;
        bool _gunBehind = false;
        #endregion

        private void Start()
        {
            // Handle the backpack extras
            if (_skinSwitcher.CurSkin == "normal-with-backpack")
            {
                _spineAnimationState.SetAnimation(GEAR_TRACK, "gear-roll", true);

                // Register effect event handler
                _effectController.OnSteamTankFull += SteamTankFullHandler;
            }

            // Get necessary bones
            _aimBone = _skeletonAnimation.Skeleton.FindBone(_aimBoneName);
            // Register controller events
            _characterController.OnAimStart += PlayAimAnim;
            _characterController.OnAimMoved += UpdateAimAnim;
            _characterController.OnAimEnd += StopAimAnim;
            _characterController.OnShoot += PlayShoot;

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

            _characterController.OnAimStart -= PlayAimAnim;
            _characterController.OnAimMoved -= UpdateAimAnim;
            _characterController.OnAimEnd -= StopAimAnim;
            _characterController.OnShoot -= PlayShoot;

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

            if (eventName == "footstep")
            {
                _audioController.PlayFootStep();
            }
            else if (eventName == "jump-up")
            {
                _characterController.Jump();
            }
            else if (eventName == "landed")
            {
                _audioController.PlayHeavyFootStep();
                _characterController.Landed();
            }
            else if (eventName == "jump-trigger-pulled")
            {
                if (OnJumpTriggerPulled != null) OnJumpTriggerPulled.Invoke();
                _audioController.PlaySteamBlastSound();
            }
            else if (eventName == "steam-ejected")
            {
                if (OnSteamEjected != null) OnSteamEjected.Invoke();
                _audioController.PlaySteamReleaseSound();
            }
            else if (eventName == "shoot")
            {
                if (OnGunShoot != null) OnGunShoot.Invoke();

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

        // Handle the aim action
        void PlayAimAnim()
        {
            // aim action
            _spineAnimationState.SetAnimation(WEAPON_TRACK, "aim", false);
            _gunBehind = false;

            UpdateAimAnim();
        }
        void UpdateAimAnim()
        {
            // Make the gun behind the body if the target is currently in the opposite position.
            if (!_gunBehind && _characterController.IsTargetOpposite())
            {
                _spineAnimationState.SetAnimation(WEAPON_TRACK, "aim-behind", false);
                _gunBehind = true;
            }
            else if (_gunBehind && !_characterController.IsTargetOpposite())
            {
                _spineAnimationState.SetAnimation(WEAPON_TRACK, "aim", false);
                _gunBehind = false;
            }

            Vector3 skeletonSpacePoint = transform.InverseTransformPoint(_characterController.AimPos);
            skeletonSpacePoint.x *= _skeletonAnimation.Skeleton.ScaleX;
            skeletonSpacePoint.y *= _skeletonAnimation.Skeleton.ScaleY;
            _aimBone.SetLocalPosition(skeletonSpacePoint);
        }
        void StopAimAnim()
        {
            _spineAnimationState.AddEmptyAnimation(WEAPON_TRACK, .1f, .1f);
            _spineAnimationState.AddEmptyAnimation(WEAPON_SECONDARY_TRACK, .1f, .1f);
        }
        void PlayShoot()
        {
            TrackEntry entry = _spineAnimationState.SetAnimation(WEAPON_SECONDARY_TRACK, "shoot", false);
            entry.AttachmentThreshold = 1;
        }
    }

}
