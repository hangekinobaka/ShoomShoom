using Spine;
using Spine.Unity;
using UnityEngine;
namespace SleepySpine
{
    public class SpineSkinSwitcher : MonoBehaviour
    {
        [Header("Base Skin settings")]
        [SpineSkin][SerializeField] protected string _baseSkin = "normal";

        protected string _curSkin;
        public string CurSkin => _curSkin;

        SkeletonAnimation _skeletonAnimation;
        Skeleton _skeleton;

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeleton = _skeletonAnimation.Skeleton;
            SetSkin(_baseSkin);
        }

        protected void SetSkin(string skin)
        {
            _curSkin = skin;

            if (_skeleton.Skin.ToString() == skin) return;

            _skeleton.SetSkin(skin);
            _skeleton.SetSlotsToSetupPose();
        }
    }
}
