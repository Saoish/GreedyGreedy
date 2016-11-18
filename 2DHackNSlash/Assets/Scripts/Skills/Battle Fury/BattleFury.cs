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


    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
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
        OC = transform.parent.parent.GetComponent<ObjectController>();
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

        else if (OC.GetType() == typeof(PlayerController)) {
            if (collider.tag == "Player") {
                if (collider.transform.parent.name == "FriendlyPlayer" || collider.transform.parent.name == "MainPlayer")
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))//Prevent duplicated attacks
                return;
            ObjectController target = collider.GetComponent<ObjectController>();
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
            ObjectController target = collider.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += DealBFSpingDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealBFSpingDMG;
            HittedStack.Push(collider);
        }
    }

    void DealBFSpingDMG(ObjectController target) {
        Value dmg = Value.CreateValue();
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

        OC.ON_MANA_UPDATE += OC.HealMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(OC.GetCurrMPH(), 1));
        OC.ON_MANA_UPDATE -= OC.HealMana;

        if (dmg.IsCrit) {
            target.ActiveOneShotVFXParticle("WeaponCritSlashVFX");
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
        if (UnityEngine.Random.value < (TriggerChance / 100)) {
            ActiveBattleFury();
        }
    }

    void ActiveBattleFury() {
        GetComponent<Animator>().speed = OC.GetAttackAnimSpeed();
        GetComponent<Animator>().SetTrigger("Active");
    }


}
