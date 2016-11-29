using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CleaveState : StateMachineBehaviour {
    float box_width = 0.3f;
    float box_height = 0.7f;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float RangeScale = animator.transform.GetComponent<Cleave>().RangeScale;
        BoxCollider2D collider = animator.transform.GetComponent<BoxCollider2D>();
        Transform T_Cleave = animator.transform;
        float off_set = box_width / 2;
        if (stateInfo.IsName("cleave_left")) {
            collider.size = new Vector2(box_width, box_height);
            collider.offset = new Vector2(-off_set, 0);
        } else if (stateInfo.IsName("cleave_right")) {
            collider.size = new Vector2(box_width, box_height);
            collider.offset = new Vector2(off_set, 0);
        } else if (stateInfo.IsName("cleave_up")) {
            collider.size = new Vector2(box_height, box_width);
            collider.offset = new Vector2(0, off_set);
        } else if (stateInfo.IsName("cleave_down")) {
            collider.size = new Vector2(box_height, box_width);
            collider.offset = new Vector2(0, -off_set);
        }
        animator.transform.GetComponent<Collider2D>().enabled = true;
        AudioSource.PlayClipAtPoint(animator.transform.GetComponent<Cleave>().SFX, animator.transform.position, GameManager.SFX_Volume);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.normalizedTime >= 0.3) {
            animator.transform.GetComponent<Collider2D>().enabled = false;
            Stack<Collider2D> HittedStack = animator.transform.GetComponent<Cleave>().HittedStack;
            if (HittedStack.Count != 0) {
                HittedStack.Clear();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
