using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GreedyNameSpace;
using System;

public abstract class Monster : ObjectController {
    public string Name;

    public bool LootDrop = true;

    public int exp;

    public int lvl;

    private Animator Anim;

    AIController AI;

    protected override void Awake() {
        base.Awake();
        AI = GetComponent<AIController>();
        Anim = VisualHolder.GetComponent<Animator>();
        VisualHolder.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Object;
    }
    // Use this for initialization
    protected override void Start() {
        base.Start();
        IniStats();

    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        ControlUpdate();
        AnimUpdate();
        Regen();
    }

    void ControlUpdate() {
        if (Stunned || !Alive) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
        } else if (Casting) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
            Direction = AI.Direction;
        } else {
            AttackVector = AI.AttackVector;
            if (HasForce()) {
                MoveVector = Vector2.zero;
            } else {
                MoveVector = AI.MoveVector;
            }
            Direction = AI.Direction;
        }
    }

    //Initialization

    void IniStats() {
        for (int i = 0; i < Stats.Size; i++) {
            CurrStats.stats[i] = MaxStats.stats[i];
        }
    }

    //----------public
    //Combat
    override public void DeductHealth(Damage dmg) {
        IC.PopUpText(dmg);
        if (dmg.Crit) {            
            Anim.Play("crit");
        }
        //if(dmg.SFX_Update)
        //        AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        
        if (CurrStats.Get(STATSTYPE.HEALTH) - dmg.Amount <= 0 && Alive) {
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
        } else {
            CurrStats.Dec(STATSTYPE.HEALTH, dmg.Amount);
        }
    }

    protected override void Die() {
        base.Die();
        SpawnEXP();
        if (LootDrop) {
            if (GetComponent<DropList>())
                GetComponent<DropList>().SpawnLoots();
        }
    }

    //Enemy Anim
    void AnimUpdate() {
        if (!Alive)
            return;
        if (Attacking) {
            Anim.speed = GetAttackAnimSpeed();
        } else {
            Anim.speed = GetMovementAnimSpeed();
        }
        if (AttackVector != Vector2.zero) {
            Attacking = true;
            Anim.SetBool("IsAttacking", true);
            Anim.SetInteger("Direction", Direction);
            Anim.SetBool("IsMoving", false);
        }
        else if (MoveVector != Vector2.zero && AttackVector == Vector2.zero) {
            Anim.SetBool("IsMoving", true);
            Anim.SetInteger("Direction", Direction);
            Anim.SetBool("IsAttacking", false);
        }
        else {
            Anim.SetBool("IsMoving", false);
            Anim.SetBool("IsAttacking", false);
        }
    }   

    protected void SpawnEXP() {        
        if (GameObject.Find("MainPlayer") != null) {
            GameObject.Find("MainPlayer").GetComponent<MainPlayer>().AddEXP(exp);
        }      
    }

    public override string GetName() {
        return Name;
    }
}
