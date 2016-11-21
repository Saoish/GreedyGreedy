using UnityEngine;
using System.Collections;
using System;

public class StandingStill : ActiveSkill {
    float Duration = 10;
    float Heal_MaxHP_Percentage;
    float DotHeal_MaxHP_Percentage;

    public AudioClip SFX;

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        StandingStilllvl SSL = null;
        switch (this.SD.lvl) {
            case 0:
                return;
            case 1:
                SSL = GetComponent<StandingStill1>();
                break;
            case 2:
                SSL = GetComponent<StandingStill2>();
                break;
            case 3:
                SSL = GetComponent<StandingStill3>();
                break;
            case 4:
                SSL = GetComponent<StandingStill4>();
                break;
            case 5:
                SSL = GetComponent<StandingStill5>();
                break;
        }
        CD = SSL.CD;
        ManaCost = SSL.ManaCost;
        Heal_MaxHP_Percentage = SSL.Heal_MaxHP_Percentage;
        DotHeal_MaxHP_Percentage = SSL.DotHeal_MaxHP_Percentage;
    }

    public override void Active() {
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;

        ActiveHeal();
        ApplyHealingBuff();
    }

    
    private void ActiveHeal() {
        Value ActiveHeal = Value.CreateValue(OC.GetMaxHealth() * (Heal_MaxHP_Percentage/100), 1);
        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(ActiveHeal);
        OC.ON_HEALTH_UPDATE -= OC.HealHP;
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }

    private void ApplyHealingBuff() {
        ModData HeallingBuffMod = ScriptableObject.CreateInstance<ModData>();
        HeallingBuffMod.Name = "HealingBuff";
        HeallingBuffMod.Duration = Duration;
        HeallingBuffMod.ModHealth = OC.GetMaxHealth() * (DotHeal_MaxHP_Percentage / 100);
        GameObject HealingBuffObject = Instantiate(Resources.Load("BuffPrefabs/" + HeallingBuffMod.Name)) as GameObject;
        HealingBuffObject.name = "HealingBuff";
        HealingBuffObject.GetComponent<Buff>().ApplyBuff(HeallingBuffMod, OC);
        RealTime_CD = CD;
    }
}
