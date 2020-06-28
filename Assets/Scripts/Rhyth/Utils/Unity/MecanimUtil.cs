using UnityEngine;

public abstract class MecanimUtil
{
    public static float GetActiveAnimationDuration(Animator animator, int layerIndex)
    {
        return animator.GetCurrentAnimatorStateInfo(layerIndex).length;
    }
}
