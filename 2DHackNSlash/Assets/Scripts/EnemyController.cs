using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyController : ObjectController {
    public AudioClip attack;
    public AudioClip hurt;
    public AudioClip die;

    public int exp;

    public string Name;
    public int lvl;

    public float MaxHealth;
    public float MaxMana;
    public float MaxAD;
    public float MaxMD;
    public float MaxAttkSpd; //Percantage
    public float MaxMoveSpd; //Percantage
    public float MaxDefense; //Percantage

    public float MaxCritChance = 0.3f; //Percantage
    public float MaxCritDmgBounus = 1f; //Percantage
    public float MaxLPH;
    public float MaxMPH;

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
    public float CurrMPH;

    private Animator Anim;

    AIController AI;

    protected override void Awake() {
        base.Awake();
        AI = GetComponent<AIController>();
        Anim = GetComponent<Animator>();
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
        CurrMPH = MaxMPH;
	}
	
	// Update is called once per frame
	void Update () {
        ControlUpdate();
        AnimUpdate();
    }

    void FixedUpdate() {
        MoveUpdate();
    }

    void MoveUpdate() {
        if (MoveVector != Vector2.zero)
            rb.MovePosition(rb.position + AI.MoveVector * (CurrMoveSpd / 100) * Time.deltaTime);
        //rb.AddForce(MoveVector * (CurrMoveSpd / 100) * rb.drag);
        //rb.velocity = MoveVector * (CurrMoveSpd / 100);
    }

    void ControlUpdate() {
        if (Stunned) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
            return;
        }
        if (AI) {
            AttackVector = AI.AttackVector;
            if (!HasForce()) {
                MoveVector = AI.MoveVector;
            } else {
                MoveVector = Vector2.zero;
            }
            Direction = AI.Direction;
        }
    }

    //----------public
    //Combat
    override public Value AutoAttackDamageDeal(float TargetDefense) {
        Value dmg = Value.CreateValue(0, 0, false, GetComponent<ObjectController>());
        if (Random.value < (CurrCritChance / 100)) {
            dmg.Amount += CurrAD * (CurrCritDmgBounus / 100);
            dmg.Amount += CurrMD * (CurrCritDmgBounus / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount = CurrAD + CurrMD;
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (TargetDefense / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;
        return dmg;
    }

    override public void DeductHealth(Value dmg) {
        if (CurrHealth - dmg.Amount <= 0) {
            CurrHealth -= dmg.Amount;
            IC.UpdateHealthBar();
            IC.PopUpText(dmg);
            DieUpdate();
            return;
        }
        if (dmg.IsCrit) {
            Animator Anim = GetComponent<Animator>();
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
        }
        AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        CurrHealth -= dmg.Amount;
        IC.UpdateHealthBar();
        IC.PopUpText(dmg);
    }

    public override void DeductMana(Value mana_cost) {
        if (CurrMana - mana_cost.Amount >= 0)//Double check
            CurrMana -= mana_cost.Amount;
        //No visual UI update
    }

    public override void HealHP(Value heal_hp) {
        if (CurrHealth < MaxHealth && CurrHealth + heal_hp.Amount <= MaxHealth) {
            CurrHealth += heal_hp.Amount;
            IC.PopUpText(heal_hp);
        } else if (CurrHealth < MaxHealth && CurrHealth + heal_hp.Amount > MaxHealth) {
            heal_hp.Amount = MaxHealth - CurrHealth;
            CurrHealth += heal_hp.Amount;
            IC.PopUpText(heal_hp);
        }
        IC.UpdateHealthBar();
    }

    public override void HealMana(Value heal_mana) {
        if (CurrMana < MaxMana && CurrMana + heal_mana.Amount <= MaxMana) {
            CurrMana += heal_mana.Amount;
            //IC.PopUpHeal(heal_mana);
        } else if (CurrMana < MaxMana && CurrMana + heal_mana.Amount > MaxMana) {
            heal_mana.Amount = MaxMana - CurrMana;
            CurrHealth += heal_mana.Amount;
            //IC.PopUpHeal(heal_mana);
        }
        IC.UpdateHealthBar();
    }


    //Animation
    override public float GetMovementAnimSpeed() {
        return (CurrMoveSpd/100) / (movement_animation_interval);
    }

    override public float GetAttackAnimSpeed() {
        return (CurrAttkSpd/100) / (attack_animation_interval);
    }

    override public float GetPhysicsSpeedFactor() {
        if (!Attacking) {
            if (CurrMoveSpd < 100)
                return 1 + CurrMoveSpd / 100;
            else if (CurrMoveSpd > 100)
                return 1 - CurrMoveSpd / 100;
            else
                return 1;
        } else {
            if (CurrAttkSpd < 100)
                return 1 + CurrAttkSpd / 100;
            else if (CurrMoveSpd > 100)
                return 1 - CurrAttkSpd / 100;
            else
                return 1;
        }
    }

    void AnimUpdate() {
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

    void SpawnEXP() {
        PlayerController MPC = GameObject.Find("MainPlayer/PlayerController").GetComponent<PlayerController>();
        if(MPC.Alive)
            MPC.AddEXP(exp);        
    }

    void DieUpdate() {
        if (CurrHealth <= 0) {//Insert dead animation here
            Alive = false;
            SpawnEXP();
            GetComponent<DropList>().SpawnLoots();
            Destroy(transform.parent.gameObject);
        }
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
    override public float GetMaxMPH() {
        return MaxMPH;
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
    override public float GetCurrMPH() {
        return CurrMPH;
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
    override public void SetMaxMPH(float mph) {
        MaxMPH = mph;
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
    override public void SetCurrMPH(float mph) {
        CurrMPH = mph;
    }
    override public void SetCurrDefense(float defense) {
        CurrDefense = defense;
    }



    public override string GetName() {
        return Name;
    }
}
