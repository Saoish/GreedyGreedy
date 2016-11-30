using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFuryState : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.GetComponent<Collider2D>().enabled = true;
        animator.transform.GetComponent<BattleFury>().Spining = true;
        //Audio source plays here
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.normalizedTime >= 0.3) {
            animator.transform.GetComponent<Collider2D>().enabled = false;
            Stack<Collider2D> HittedStack = animator.transform.GetComponent<BattleFury>().HittedStack;
            if (HittedStack.Count != 0) {
                HittedStack.Clear();
            }
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.GetComponent<BattleFury>().Spining = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
