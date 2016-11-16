using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemyAttackState : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        MeleeAttackEnter(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.normalizedTime >= 0.5) {
            MeleeAttacExit(animator, stateInfo, layerIndex);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        EnemyController EC = animator.gameObject.GetComponent<EnemyController>();
        EC.Attacking = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    void MeleeAttackEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        EnemyController EC = animator.transform.GetComponent<EnemyController>();
        Transform T_AttackCollider = animator.transform.Find("MeleeAttackCollider");
        EC.Attacking = true;
        BoxCollider2D AttackCollider = T_AttackCollider.GetComponent<BoxCollider2D>();
        float AttackRange = T_AttackCollider.GetComponent<MeleeEnemeyAttackCollider>().AttackRange;
        float AttackBoxWidth = T_AttackCollider.GetComponent<MeleeEnemeyAttackCollider>().AttackBoxWidth;
        float AttackBoxHeight = T_AttackCollider.GetComponent<MeleeEnemeyAttackCollider>().AttackBoxHeight;
        if (stateInfo.IsName("attack_left")) {
            AttackCollider.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            AttackCollider.offset = new Vector2(-AttackRange, 0);
        } else if (stateInfo.IsName("attack_right")) {
            AttackCollider.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            AttackCollider.offset = new Vector2(AttackRange, 0);
        } else if (stateInfo.IsName("attack_up")) {
            AttackCollider.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            AttackCollider.offset = new Vector2(0, AttackRange);
        } else if (stateInfo.IsName("attack_down")) {
            AttackCollider.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            AttackCollider.offset = new Vector2(0, -AttackRange);
        }
        AttackCollider.enabled = true;
        AudioSource.PlayClipAtPoint(EC.attack, animator.transform.position, GameManager.SFX_Volume);
    }

    void MeleeAttacExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        EnemyController EC = animator.gameObject.GetComponent<EnemyController>();
        Transform T_AttackCollider = animator.transform.Find("MeleeAttackCollider");
        BoxCollider2D AttackCollider = T_AttackCollider.GetComponent<BoxCollider2D>();
        AttackCollider.enabled = false;
        Stack<Collider2D> HittedStack = T_AttackCollider.GetComponent<MeleeEnemeyAttackCollider>().HittedStack;
        if (HittedStack.Count != 0)
            HittedStack.Clear();
    }
}
