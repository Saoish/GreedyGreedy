using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;
public class StandingStill : ActiveSkill {
    public float Duration = 10;
    float Heal_MaxHP_Percentage;
    float DotHeal_MaxHP_Percentage;

    public AudioClip SFX;

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
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

        Description = "Instantly gain "+Heal_MaxHP_Percentage+"% of your MAX HP back and a healing buff to heal "+DotHeal_MaxHP_Percentage+"% of your MAX HP you every second for "+Duration+" secs.\n\nCost: "+ManaCost+" Mana\nCD: "+CD+" secs";
    }

    public override void Active() {
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(new Value(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;

        ActiveHeal();
        ApplyHealingBuff();
        RealTime_CD = CD;
    }

    
    private void ActiveHeal() {
        Value ActiveHeal = new Value(OC.GetMaxStats(StatsType.HEALTH) * (Heal_MaxHP_Percentage/100), 1);
        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(ActiveHeal);
        OC.ON_HEALTH_UPDATE -= OC.HealHP;
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }

    private void ApplyHealingBuff() {
        HealingBuff HB = HealingBuff.Generate(OC.GetMaxStats(StatsType.HEALTH) * (DotHeal_MaxHP_Percentage / 100), Duration);
        HB.ApplyBuff(OC);
    }
}
