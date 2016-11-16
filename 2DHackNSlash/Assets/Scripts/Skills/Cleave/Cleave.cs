using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Cleave : ActiveSkill {
    [HideInInspector]
    public float ADScale;
    [HideInInspector]
    public float RangeScale;
    [HideInInspector]
    public Animator Anim;

    public AudioClip SFX;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        Anim = GetComponent<Animator>();
        if (transform.parent == null)
            return;
    }

    protected override void Start() {
        base.Start();
    }


    protected override void Update() {
        base.Update();
        
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        Cleavelvl CL = null;
        switch (this.SD.lvl) {
            case 0:
                return;
            case 1:
                CL = GetComponent<Cleave1>();
                break;
            case 2:
                CL = GetComponent<Cleave2>();
                break;
            case 3:
                CL = GetComponent<Cleave3>();
                break;
            case 4:
                CL = GetComponent<Cleave4>();
                break;
            case 5:
                CL = GetComponent<Cleave5>();
                break;
        }
        CD = CL.CD;
        ManaCost = CL.ManaCost;
        ADScale = CL.ADScale;
        RangeScale = CL.RangeScale;
        transform.localScale = new Vector2(RangeScale, RangeScale);
        OC = transform.parent.parent.GetComponent<ObjectController>();
    }

    public override bool Ready() {
        if (OC.Stunned) {
            Debug.Log(SD.Name + " " + SD.lvl + ": You are Stunned");
            return false;
        }
        else if (RealTime_CD > 0) {
            Debug.Log(SD.Name + " " + SD.lvl + ": Is on cooldown");
            return false;
        }
        else if (OC.GetCurrMana() - ManaCost < 0) {
            Debug.Log(SD.Name + " " + SD.lvl + ": Not enough mana");
            return false;
        }
        return true;
    }

    public override void Active() {
        Anim.SetInteger("Direction", OC.Direction);
        Anim.SetTrigger("Active");
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;
        RealTime_CD = CD;
    }

    //Unique Methods

    void DealSkillDmg(ObjectController target) {
        Value dmg = Value.CreateValue();
        if (UnityEngine.Random.value < (OC.GetCurrCritChance() / 100)) {
            dmg.Amount += OC.GetCurrAD() * (ADScale/100) * (OC.GetCurrCritDmgBounus() / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount += OC.GetCurrAD() * (ADScale / 100);
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (target.GetCurrDefense() / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;

        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(Value.CreateValue(OC.GetCurrLPH(), 1));
        OC.ON_HEALTH_UPDATE -= OC.HealHP;

        OC.ON_MANA_UPDATE += OC.HealMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(OC.GetCurrMPH(), 1));
        OC.ON_MANA_UPDATE -= OC.HealMana;

        if (dmg.IsCrit) {
            target.ActiveOneShotVFXParticle("WeaponCritSlashVFX");
        }

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Enemy") {
            if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            EnemyController Enemy = collider.GetComponent<EnemyController>();
            Vector2 BouceOffDirection = (Vector2)Vector3.Normalize(Enemy.transform.position - OC.transform.position);
            Enemy.rb.mass = 1;
            Enemy.rb.AddForce(BouceOffDirection * SD.lvl * 2, ForceMode2D.Impulse);
            OC.ON_DMG_DEAL += DealSkillDmg;
            OC.ON_DMG_DEAL(Enemy);
            OC.ON_DMG_DEAL -= DealSkillDmg;
            HittedStack.Push(collider);
        } else if (collider.transform.tag == "Player") {

        }
    }
}
