using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;
using System;

public class BattleFury : PassiveSkill {
    public GameObject HitVFX;
    public AudioClip HitSFX;

    [HideInInspector]
    public float TriggerChance;
    [HideInInspector]
    public float Dot_DamageSCale_Percentage;
    [HideInInspector]
    public float Sping_DamageScale;

    public float BleedDuration = 10;

    [HideInInspector]
    public bool Spining = false;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    string DescriptionTemplate(BattleFurylvl[] AllLvls, int Index) {  
        return "\nUpon dealing damage, you have " + MyText.Colofied(AllLvls[Index].TriggerChance + "%", highlight) + " chance to summon a weapon dealing " + MyText.Colofied(AllLvls[Index].Sping_DamageScale + "%", highlight) + " damage to enemies around you and apply bleeding on them for " + BleedDuration + " secs to bleed " + MyText.Colofied(AllLvls[Index].Dot_DamageScale_Percentage + "%", highlight) + " damage/sec.";
    }

    public override void GenerateDescription() {
        BattleFurylvl[] AllLvls = GetComponents<BattleFurylvl>();
        Description = "Level: " + lvl + "/" + Patch.MaxSkilllvl;
        if(lvl == 0) {
            Description += DescriptionTemplate(AllLvls, 0);
        } else {
            Description += DescriptionTemplate(AllLvls, lvl-1);
            if (lvl == Patch.MaxSkilllvl)
                return;
            Description += "\n\nNext Level:";
            Description += DescriptionTemplate(AllLvls, lvl);
        }
    }

    protected override void Awake() {
        base.Awake();
        GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Skill;
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        BattleFurylvl BFL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                BFL = GetComponent<BattleFury1>();
                break;
            case 2:
                BFL = GetComponent<BattleFury2>();
                break;
            case 3:
                BFL = GetComponent<BattleFury3>();
                break;
            case 4:
                BFL = GetComponent<BattleFury4>();
                break;
            case 5:
                BFL = GetComponent<BattleFury5>();
                break;
        }
        TriggerChance = BFL.TriggerChance;
        Sping_DamageScale = BFL.Sping_DamageScale;
        Dot_DamageSCale_Percentage = BFL.Dot_DamageScale_Percentage;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), OC.GetRootCollider());//Ignore self here
        GenerateDescription();
        
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_DMG_DEAL+=ApplyBattlFuryPassive;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer(CollisionLayer.KillingGround))
            return;

        if (OC.GetType().IsSubclassOf(typeof(Player))) {//Player Attack
            if (collider.tag == Tag.FriendlyPlayer) {
                //if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))//Prevent duplicated attacks
                return;
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            OC.ON_DMG_DEAL += DealBFSpingDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealBFSpingDMG;
            HittedStack.Push(collider);
        } else {
            if (collider.tag == Tag.Monster) {
                return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            OC.ON_DMG_DEAL += DealBFSpingDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealBFSpingDMG;
            HittedStack.Push(collider);
        }
    }

    void DealBFSpingDMG(ObjectController target) {
        float RawDamage;
        bool Crit;        
        if (UnityEngine.Random.value < (OC.GetCurrStats(STATSTYPE.CRIT_CHANCE) / 100)) {
            RawDamage = OC.CurrDamage * (Sping_DamageScale / 100) * (OC.CurrCritChance / 100);
            Crit = true;
        } else {
            RawDamage = OC.CurrDamage * (Sping_DamageScale / 100);
            Crit = false;
        }
        DirectDamage SpingDmg = new DirectDamage(RawDamage, target.CurrDefense, OC.CurrPenetration, Crit, OC, typeof(BattleFury));

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(new Value(OC.GetCurrLPH(), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ActiveOneShotVFXParticle(HitVFX);

        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(SpingDmg);
        target.ON_DMG_TAKEN -= target.DeductHealth;

        if (!target.HasDebuff(typeof(BleedDebuff)))
            ApplyBleedDebuff(target);
    }


    //Private
    void ApplyBleedDebuff(ObjectController target) {
        BleedDebuff BD = BleedDebuff.Generate(OC.GetCurrStats(STATSTYPE.DAMAGE) * (Dot_DamageSCale_Percentage / 100), BleedDuration);
        BD.ApplyDebuff(OC,target);
    }

    void ApplyBattlFuryPassive(ObjectController target) {
        if (!Spining && UnityEngine.Random.value < (TriggerChance / 100)) {
            ActiveBattleFury();
        }
    }

    void ActiveBattleFury() {
        GetComponent<Animator>().speed = OC.GetAttackAnimSpeed();
        GetComponent<Animator>().SetTrigger("Active");
    }


}
