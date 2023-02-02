using Spine.Unity;
using UnityEngine;

namespace SleepySpine
{
    public class SpineAnimationController : MonoBehaviour
    {
        protected SkeletonAnimation _skeletonAnimation;
        protected Spine.AnimationState _spineAnimationState;

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _spineAnimationState = _skeletonAnimation.AnimationState;
        }


    }
}