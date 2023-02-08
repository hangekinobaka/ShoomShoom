using Spine.Unity;
using UnityEngine;

namespace SleepySpine
{
    public class SpineAnimationController : MonoBehaviour
    {
        protected SkeletonAnimation _skeletonAnimation;
        protected Spine.AnimationState _spineAnimationState;

        void OnValidate()
        {
            if (_skeletonAnimation == null) _skeletonAnimation = GetComponent<SkeletonAnimation>();
        }
        private void Awake()
        {
            if (_skeletonAnimation == null) _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _spineAnimationState = _skeletonAnimation.AnimationState;
        }


    }
}