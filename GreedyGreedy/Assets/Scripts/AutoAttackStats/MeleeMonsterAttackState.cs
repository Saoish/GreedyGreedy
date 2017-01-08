using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeMonsterAttackState : AutoAttackState {
    public AudioClip melee_attack_sfx;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        MeleeAttackEnter(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Monster EC = animator.transform.parent.parent.GetComponent<Monster>();        
        if(EC.Stunned)
            animator.CrossFade("Entrance", 0);
        else if (stateInfo.normalizedTime >= EC.Melee_AC.BoxTrigger.Start && stateInfo.normalizedTime< EC.Melee_AC.BoxTrigger.End) {
            EC.Melee_AC.Active();            
        }
        else if(stateInfo.normalizedTime >= EC.Melee_AC.BoxTrigger.End)
            EC.Melee_AC.Deactive();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Monster EC = animator.transform.parent.parent.GetComponent<Monster>();
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
        Monster EC = animator.transform.parent.parent.GetComponent<Monster>();
        EC.Attacking = true;                
        float AttackRange = EC.Melee_AC.AttackRange;
        float AttackBoxWidth = EC.Melee_AC.AttackBoxWidth;
        float AttackBoxHeight = EC.Melee_AC.AttackBoxHeight;
        if (stateInfo.IsName("attack_left")) {
            EC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            EC.Melee_AC.MeleeCollder.offset = new Vector2(-AttackRange, 0);
        } else if (stateInfo.IsName("attack_right")) {
            EC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            EC.Melee_AC.MeleeCollder.offset = new Vector2(AttackRange, 0);
        } else if (stateInfo.IsName("attack_up")) {
            EC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            EC.Melee_AC.MeleeCollder.offset = new Vector2(0, AttackRange);
        } else if (stateInfo.IsName("attack_down")) {
            EC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            EC.Melee_AC.MeleeCollder.offset = new Vector2(0, -AttackRange);
        }
        if(melee_attack_sfx!=null)
            AudioSource.PlayClipAtPoint(melee_attack_sfx, animator.transform.position, GameManager.SFX_Volume);
    }
}
