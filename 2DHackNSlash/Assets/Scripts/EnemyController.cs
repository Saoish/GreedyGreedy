using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GreedyNameSpace;
using System;

public abstract class EnemyController : ObjectController {
    public string Name;

    public bool LootDrop = true;

    public AudioClip hurt;

    public int exp;

    public int lvl;

    public float HEALTH = 100;
    public float MANA = 100;
    public float AD;
    public float MD;
    public float ATTACK_SPEED; //Percantage
    public float MOVE_SPEED; //Percantage
    public float DEFENSE; //Percantage

    public float CRIT_CHANCE = 10f; //Percantage
    public float CRIT_DMG = 200f; //Percantage
    public float LPH;
    public float HEALTH_REGEN = 0;
    public float MANA_REGEN = 10;
    public float CDR = 0;

    private Animator Anim;

    AIController AI;

    protected override void Awake() {
        base.Awake();
        AI = GetComponent<AIController>();
        Anim = VisualHolder.GetComponent<Animator>();
        VisualHolder.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Object;
    }
    // Use this for initialization
    protected override void Start() {
        base.Start();
        InitMaxStats();
        InitCurrStats();

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
            return;
        }
        else {
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
    void InitMaxStats() {
        MaxStats.Set(StatsType.HEALTH, HEALTH);
        MaxStats.Set(StatsType.MANA, MANA);
        MaxStats.Set(StatsType.AD, AD);
        MaxStats.Set(StatsType.MD, MD);
        MaxStats.Set(StatsType.ATTACK_SPEED, ATTACK_SPEED);
        MaxStats.Set(StatsType.MOVE_SPEED, MOVE_SPEED);
        MaxStats.Set(StatsType.DEFENSE, DEFENSE);
        MaxStats.Set(StatsType.CRIT_CHANCE, CRIT_CHANCE);
        MaxStats.Set(StatsType.CRIT_DMG, CRIT_DMG);
        MaxStats.Set(StatsType.LPH, LPH);
        MaxStats.Set(StatsType.HEALTH_REGEN, HEALTH_REGEN);
        MaxStats.Set(StatsType.MANA_REGEN, MANA_REGEN);
        MaxStats.Set(StatsType.CDR, CDR);
    }

    void InitCurrStats() {
        for (int i = 0; i < Stats.Size; i++) {
            CurrStats.Set(i, MaxStats.Get(i));
        }
    }

    //----------public
    //Combat
    override public void DeductHealth(Value dmg) {
        if(dmg.Pop_Update)
            IC.PopUpText(dmg);
        if (dmg.IsCrit) {
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
        }
        if(dmg.SFX_Update)
                AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        
        if (CurrStats.Get(StatsType.HEALTH) - dmg.Amount <= 0 && Alive) {
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
        } else {
            CurrStats.Dec(StatsType.HEALTH, dmg.Amount);
        }
    }

    protected override void Die() {
        base.Die();
        SpawnEXP();
        if (LootDrop) {
            if(GetComponent<DropList>())
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
        //MainPlayer MPC = GameObject.Find("MainPlayer").GetComponent<MainPlayer>();
        if (GameObject.Find("MainPlayer") != null) {
            GameObject.Find("MainPlayer").GetComponent<MainPlayer>().AddEXP(exp);
        }      
    }

    public override string GetName() {
        return Name;
    }
}
