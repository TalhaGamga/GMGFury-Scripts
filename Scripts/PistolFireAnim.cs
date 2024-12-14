using UnityEngine;

public class PistolFireAnim : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Fired", true);
        animator.SetBool("Firable", false);
    }
}
