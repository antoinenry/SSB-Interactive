using UnityEngine;

public class IntroStage : Stage
{
    public Animation logoAnimation;
    public AnimationClip[] animations;

    public override int MomentCount => AnimationCount;
    public int AnimationCount => animations != null ? animations.Length : 0;

    protected override void OnMomentChange(int value)
    {
        base.OnMomentChange(value);
        if (logoAnimation != null && value >= 0 && value < AnimationCount)
        {
            logoAnimation.clip = animations[value];
            logoAnimation.Play();
        }
    }
}
