using Spine;
using Spine.Unity;
namespace SleepySpine
{
    static public class SpineAnimationUtil
    {
        /// <summary>
        /// Sets the horizontal flip state of the skeleton based on a nonzero float. <br/>
        /// If negative, the skeleton is flipped. If positive, the skeleton is not flipped.
        /// </summary>
        static public void SetFlip(this SkeletonAnimation skeletonAnimation, float horizontal)
        {
            if (horizontal != 0)
            {
                skeletonAnimation.Skeleton.ScaleX = horizontal > 0 ? 1f : -1f;
            }
        }

        static public TrackEntry GetCurrentEntry(this SkeletonAnimation skeletonAnimation, int layerIndex)
        {
            TrackEntry trackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
            return trackEntry != null ? trackEntry : null;
        }

    }
}