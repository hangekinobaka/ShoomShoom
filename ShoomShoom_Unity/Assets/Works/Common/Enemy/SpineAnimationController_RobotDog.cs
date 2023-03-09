
namespace SleepySpine
{
    public class SpineAnimationController_RobotDog : SpineAnimationController_Enemy
    {
        const int STOVE_TRACK = 10;
        protected override void Start()
        {
            base.Start();

            // Add fire stove anim
            _spineAnimationState.SetAnimation(STOVE_TRACK, "stove-fire", true);
        }

    }
}