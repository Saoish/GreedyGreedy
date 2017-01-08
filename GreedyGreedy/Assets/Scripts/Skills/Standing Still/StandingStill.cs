using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;
public class StandingStill : ActiveSkill {
    public float Duration = 10;
    float Heal_MaxHP_Percentage;
    float DotHeal_MaxHP_Percentage;

    public AudioClip SFX;

    string DescriptionTemplate(StandingStilllvl[] AllLvls, int Index) {
        return "\nInstantly gain " + MyText.Colofied(AllLvls[Index].Heal_MaxHP_Percentage+"%",highlight) + " of your MAX HP back and a healing buff to heal " + MyText.Colofied(AllLvls[Index].DotHeal_MaxHP_Percentage + "%", highlight) + " of your MAX HP you every second for " + Duration + " secs.\n\nCost: " + MyText.Colofied(AllLvls[Index].EssenseCost + " Essense", highlight) + "\nCD: " + MyText.Colofied(AllLvls[Index].CD + " secs", highlight);
    }

    public override void GenerateDescription() {
        StandingStilllvl[] AllLvls = GetComponents<StandingStilllvl>();
        Description = "Level: " + lvl + "/" + Patch.MaxSkilllvl;
        if (lvl == 0) {
            Description += DescriptionTemplate(AllLvls, 0);
        } else {
            Description += DescriptionTemplate(AllLvls, lvl - 1);
            if (lvl == Patch.MaxSkilllvl)
                return;
            Description += "\n\nNext Level:";
            Description += DescriptionTemplate(AllLvls, lvl);
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        StandingStilllvl SSL = null;
        switch (this.lvl) {
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
        EssenseCost = SSL.EssenseCost;
        Heal_MaxHP_Percentage = SSL.Heal_MaxHP_Percentage;
        DotHeal_MaxHP_Percentage = SSL.DotHeal_MaxHP_Percentage;

        GenerateDescription();
    }

    public override void Active() {
        OC.ON_ESSENSE_COST += OC.DeductEssense;
        OC.ON_ESSENSE_COST(new EssenseCost(EssenseCost,OC,typeof(StandingStill)));
        OC.ON_ESSENSE_COST -= OC.DeductEssense;

        ActiveHeal();
        ApplyHealingBuff();
        RealTime_CD =  CD - CD * (OC.GetMaxStats(STATSTYPE.HASTE) / 100);
    }

    
    private void ActiveHeal() {
        HealHP Heal = new HealHP(OC.MaxHealth * (Heal_MaxHP_Percentage/100),false,OC,typeof(StandingStill));
        OC.ON_HEALTH_GAIN += OC.HealHP;
        OC.ON_HEALTH_GAIN(Heal);
        OC.ON_HEALTH_GAIN -= OC.HealHP;
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }

    private void ApplyHealingBuff() {
        HealingBuff HB = HealingBuff.Generate(OC.GetMaxStats(STATSTYPE.HEALTH) * (DotHeal_MaxHP_Percentage / 100), Duration);
        HB.ApplyBuff(OC,OC);
    }
}
