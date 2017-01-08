using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;

public class GreatSwordAttackState : AutoAttackState {

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        MeleeAttackEnter(animator, stateInfo);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Player PC = animator.transform.parent.parent.parent.GetComponent<Player>();
        if (PC.Stunned)
            animator.CrossFade("Entrance", 0);
        else if (stateInfo.normalizedTime >= PC.Melee_AC.BoxTrigger.Start && stateInfo.normalizedTime < PC.Melee_AC.BoxTrigger.End) {
            PC.Melee_AC.Active();
        } else if (stateInfo.normalizedTime >= PC.Melee_AC.BoxTrigger.End)
            PC.Melee_AC.Deactive();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Player PC = animator.transform.parent.parent.parent.GetComponent<Player>();
        PC.Attacking = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    //}

    //OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK(inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    //}

    void MeleeAttackEnter(Animator animator, AnimatorStateInfo stateInfo) {
        Player PC = animator.transform.parent.parent.parent.GetComponent<Player>();
        WeaponController WC = PC.GetWC();
        PC.ON_ESSENSE_COST += PC.DeductEssense;
        PC.ON_ESSENSE_COST(new EssenseCost(WC.EssenseCost,PC,typeof(GreatSwordAttackState)));
        PC.ON_ESSENSE_COST -= PC.DeductEssense;
        PC.Attacking = true;
        float AttackRange = PC.Melee_AC.AttackRange;
        float AttackBoxWidth = PC.Melee_AC.AttackBoxWidth;
        float AttackBoxHeight = PC.Melee_AC.AttackBoxHeight;

        //Combo 1
        if (stateInfo.IsName("combo1_left")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(-AttackRange, 0);
            AudioSource.PlayClipAtPoint(WC.combo_1, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo1_right")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(AttackRange, 0);
            AudioSource.PlayClipAtPoint(WC.combo_1, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo1_up")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(0, AttackRange);
            AudioSource.PlayClipAtPoint(WC.combo_1, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo1_down")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(0, -AttackRange);
            AudioSource.PlayClipAtPoint(WC.combo_1, animator.transform.position, GameManager.SFX_Volume);
        }
        //Combo 2
        else if (stateInfo.IsName("combo2_left")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(-AttackRange, 0);
            AudioSource.PlayClipAtPoint(WC.combo_2, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo2_right")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxWidth, AttackBoxHeight);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(AttackRange, 0);
            AudioSource.PlayClipAtPoint(WC.combo_2, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo2_up")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(0, AttackRange);
            AudioSource.PlayClipAtPoint(WC.combo_2, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo2_down")) {
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackBoxHeight, AttackBoxWidth);
            PC.Melee_AC.MeleeCollder.offset = new Vector2(0, -AttackRange);
            AudioSource.PlayClipAtPoint(WC.combo_2, animator.transform.position, GameManager.SFX_Volume);
        }
        //Combo 3
        else if (stateInfo.IsName("combo3_left")) {
            PC.Melee_AC.MeleeCollder.offset = Vector2.zero;
            float AttackRadius = AttackBoxHeight >= AttackBoxWidth ? AttackBoxWidth + AttackRange * 2 : AttackBoxHeight + AttackRange * 2;
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackRadius, AttackRadius);
            AudioSource.PlayClipAtPoint(WC.combo_3, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo3_right")) {
            PC.Melee_AC.MeleeCollder.offset = Vector2.zero;
            float AttackRadius = AttackBoxHeight >= AttackBoxWidth ? AttackBoxWidth + AttackRange * 2 : AttackBoxHeight + AttackRange * 2;
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackRadius, AttackRadius);
            AudioSource.PlayClipAtPoint(WC.combo_3, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo3_up")) {
            PC.Melee_AC.MeleeCollder.offset = Vector2.zero;
            float AttackRadius = AttackBoxHeight >= AttackBoxWidth ? AttackBoxWidth + AttackRange * 2 : AttackBoxHeight + AttackRange * 2;
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackRadius, AttackRadius);
            AudioSource.PlayClipAtPoint(WC.combo_3, animator.transform.position, GameManager.SFX_Volume);
        } else if (stateInfo.IsName("combo3_down")) {
            PC.Melee_AC.MeleeCollder.offset = Vector2.zero;
            float AttackRadius = AttackBoxHeight >= AttackBoxWidth ? AttackBoxWidth + AttackRange * 2 : AttackBoxHeight + AttackRange * 2;
            PC.Melee_AC.MeleeCollder.size = new Vector2(AttackRadius, AttackRadius);        
            AudioSource.PlayClipAtPoint(WC.combo_3, animator.transform.position, GameManager.SFX_Volume);
        }
    }
}
