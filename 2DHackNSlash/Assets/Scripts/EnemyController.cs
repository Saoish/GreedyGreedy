using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class EnemyController : ObjectController {
    public bool LootDrop = true;

    public AudioClip hurt;

    public int exp;

    public string Name;
    public int lvl;

    public float MaxHealth;
    public float MaxMana = 100;
    public float MaxAD;
    public float MaxMD;
    public float MaxAttkSpd; //Percantage
    public float MaxMoveSpd; //Percantage
    public float MaxDefense; //Percantage

    public float MaxCritChance = 30f; //Percantage
    public float MaxCritDmgBounus = 200f; //Percantage
    public float MaxLPH;
    public float MaxManaRegen = 5;

    [HideInInspector]
    public float CurrHealth;
    [HideInInspector]
    public float CurrMana;
    [HideInInspector]
    public float CurrAD;
    [HideInInspector]
    public float CurrMD;
    [HideInInspector]
    public float CurrAttkSpd;
    [HideInInspector]
    public float CurrMoveSpd;
    [HideInInspector]
    public float CurrDefense;
    [HideInInspector]
    public float CurrCritChance;
    [HideInInspector]
    public float CurrCritDmgBounus;
    [HideInInspector]
    public float CurrLPH;
    [HideInInspector]
    public float CurrManaRegen;

    private Animator Anim;

    AIController AI;

    protected override void Awake() {
        base.Awake();
        AI = GetComponent<AIController>();
        Anim = VisualHolder.GetComponent<Animator>();
        VisualHolder.GetComponent<SpriteRenderer>().sortingOrder = Layer.OJ;
    }
    // Use this for initialization
    protected override void Start() {
        base.Start();
        CurrHealth = MaxHealth;
        CurrMana = MaxMana;
        CurrAD = MaxAD;
        CurrMD = MaxMD;
        CurrAttkSpd = MaxAttkSpd;
        CurrMoveSpd = MaxMoveSpd;
        CurrDefense = MaxDefense;

        CurrCritChance = MaxCritChance;
        CurrCritDmgBounus = MaxCritDmgBounus;

        CurrLPH = MaxLPH;
        CurrManaRegen = MaxManaRegen;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        ControlUpdate();
        AnimUpdate();
        ManaRegen();
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

    //----------public
    //Combat
    override public void DeductHealth(Value dmg) {
        if (dmg.IsCrit) {
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
        } else {
            if(dmg.Type!=-1)
                AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        }
        if (CurrHealth - dmg.Amount <= 0 && Alive) {
            IC.PopUpText(dmg);
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
        } else {
            CurrHealth -= dmg.Amount;
            IC.PopUpText(dmg);
        }
    }

    protected override void Die() {
        base.Die();
        SpawnEXP();
        if(LootDrop)
            GetComponent<DropList>().SpawnLoots();
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
        MainPlayer MPC = GameObject.Find("MainPlayer/PlayerController").GetComponent<MainPlayer>();
        if(MPC.Alive)
            MPC.AddEXP(exp);        
    }










    override public float GetMaxHealth() {
        return MaxHealth;
    }
    override public float GetMaxMana() {
        return MaxMana;
    }
    override public float GetMaxAD() {
        return MaxAD;
    }
    override public float GetMaxMD() {
        return MaxMD;
    }
    override public float GetMaxAttkSpd() {
        return MaxAttkSpd;
    }
    override public float GetMaxMoveSpd() {
        return MaxMoveSpd;
    }
    override public float GetMaxCritChance() {
        return MaxCritChance;
    }
    override public float GetMaxCritDmgBounus() {
        return MaxCritDmgBounus;
    }
    override public float GetMaxLPH() {
        return MaxLPH;
    }
    override public float GetMaxManaRegen() {
        return MaxManaRegen;
    }
    override public float GetMaxDefense() {
        return MaxDefense;
    }





    override public float GetCurrHealth() {
        return CurrHealth;
    }
    override public float GetCurrMana() {
        return CurrMana;
    }
    override public float GetCurrAD() {
        return CurrAD;
    }
    override public float GetCurrMD() {
        return CurrMD;
    }
    override public float GetCurrAttkSpd() {
        return CurrAttkSpd;
    }
    override public float GetCurrMoveSpd() {
        return CurrMoveSpd;
    }
    override public float GetCurrCritChance() {
        return CurrCritChance;
    }
    override public float GetCurrCritDmgBounus() {
        return CurrCritDmgBounus;
    }
    override public float GetCurrLPH() {
        return CurrLPH;
    }
    override public float GetCurrManaRegen() {
        return CurrManaRegen;
    }
    override public float GetCurrDefense() {
        return CurrDefense;
    }



    override public void SetMaxHealth(float health) {
        MaxHealth = health;
    }
    override public void SetMaxMana(float mana) {
        MaxMana = mana;
    }
    override public void SetMaxAD(float ad) {
        MaxAD = ad;
    }
    override public void SetMaxMD(float md) {
        MaxMD = md;
    }
    override public void SetMaxAttkSpd(float attkspd) {
        MaxAttkSpd = attkspd;
    }
    override public void SetMaxMoveSpd(float movespd) {
        MaxMoveSpd = movespd;
    }
    override public void SetMaxCritChance(float critchance) {
        MaxCritChance = critchance;
    }
    override public void SetMaxCritDmgBounus(float critdmg) {
        MaxCritDmgBounus = critdmg;
    }
    override public void SetMaxLPH(float lph) {
        MaxLPH = lph;
    }
    override public void SetMaxManaRegen(float mph) {
        MaxManaRegen = mph;
    }
    override public void SetMaxDefense(float defense) {
        MaxDefense = defense;
    }


    override public void SetCurrHealth(float health) {
        CurrHealth = health;
    }
    override public void SetCurrMana(float mana) {
        CurrMana = mana;
    }
    override public void SetCurrAD(float ad) {
        CurrAD = ad;        
    }
    override public void SetCurrMD(float md) {
        CurrMD = md;
    }
    override public void SetCurrAttkSpd(float attkspd) {
        CurrAttkSpd = attkspd;
    }
    override public void SetCurrMoveSpd(float movespd) {
        CurrMoveSpd = movespd;
    }
    override public void SetCurrCritChance(float critchance) {
        CurrCritChance = critchance;
    }
    override public void SetCurrCritDmgBounus(float critdmg) {
        CurrCritDmgBounus = critdmg;
    }
    override public void SetCurrLPH(float lph) {
        CurrLPH = lph;
    }
    override public void SetCurrManaRegen(float mph) {
        CurrManaRegen = mph;
    }
    override public void SetCurrDefense(float defense) {
        CurrDefense = defense;
    }



    public override string GetName() {
        return Name;
    }
}
