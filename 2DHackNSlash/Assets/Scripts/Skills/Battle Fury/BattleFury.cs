using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFury : PassiveSkill {
    public AudioClip BF_SFX;

    [HideInInspector]
    public float TriggerChance;
    [HideInInspector]
    public float Dot_ADSCale_Percentage;
    [HideInInspector]
    public float Sping_ADScale;

    public int MaxStack = 3;
    public float BleedDuration = 10;

    [HideInInspector]
    public bool Spining = false;


    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        GetComponent<SpriteRenderer>().sortingOrder = Layer.Skill;
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        BattleFurylvl BFL = null;
        switch (this.SD.lvl) {
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
        Sping_ADScale = BFL.Sping_ADScale;
        Dot_ADSCale_Percentage = BFL.Dot_ADScale_Percentage;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), OC.GetRootCollider());//Ignore self here

        Description = "Upon dealing damage, you have "+ TriggerChance+"% chance to summon a weapon dealing "+ Sping_ADScale+"% AD dmg to enemies around you and apply bleeding on them for "+BleedDuration+" secs to bleed "+Dot_ADSCale_Percentage+"% AD dmg per sec with max stack of "+MaxStack+".";
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
        if (collider.gameObject.layer != LayerMask.NameToLayer("KillingGround"))
            return;

        if (OC.GetType().IsSubclassOf(typeof(PlayerController))) {//Player Attack
            if (collider.tag == "Player") {
                if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))//Prevent duplicated attacks
                return;
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            OC.ON_DMG_DEAL += DealBFSpingDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealBFSpingDMG;
            HittedStack.Push(collider);
        } else {
            if (collider.tag == "Enemy") {
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
        Value dmg = Value.CreateValue(0, -1, false, OC);
        if (UnityEngine.Random.value < (OC.GetCurrCritChance() / 100)) {
            dmg.Amount += OC.GetCurrAD() * (Sping_ADScale / 100) * (OC.GetCurrCritDmgBounus() / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount += OC.GetCurrAD() * (Sping_ADScale / 100);
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (target.GetCurrDefense() / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;

        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(Value.CreateValue(OC.GetCurrLPH(), 1));
        OC.ON_HEALTH_UPDATE -= OC.HealHP;

        if (dmg.IsCrit) {
            target.ActiveOneShotVFXParticle("WeaponCritSlashVFX", Layer.Skill);
        }

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;

        if (target.DebuffStack(typeof(BleedDebuff)) < MaxStack)
            ApplyBleedDebuff(target);
    }


    //Private
    void ApplyBleedDebuff(ObjectController target) {
        ModData BleedDebuffMod = ScriptableObject.CreateInstance<ModData>();
        BleedDebuffMod.Name = "BleedDebuff";
        BleedDebuffMod.Duration = BleedDuration;
        BleedDebuffMod.ModHealth = OC.GetCurrAD() * (Dot_ADSCale_Percentage / 100);
        GameObject BleedDebuffObject = Instantiate(Resources.Load("DebuffPrefabs/" + BleedDebuffMod.Name)) as GameObject;
        BleedDebuffObject.name = "BleedDebuff";
        BleedDebuffObject.GetComponent<Debuff>().ApplyDebuff(BleedDebuffMod, target);
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
