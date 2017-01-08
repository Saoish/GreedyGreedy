using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class Retaliation : PassiveSkill {

    float TriggerChance;
    float Reflected_DMG_Percentage;

    string DescriptionTemplate(Retaliationlvl[] AllLvls, int Index) {
        return "\nUpon taking damage, you have " + MyText.Colofied(AllLvls[Index].TriggerChance +"%",highlight) + " chance to reflect " + MyText.Colofied(AllLvls[Index].Reflected_DMG_Percentage + "%", highlight) + " taken damge to attacker as true damage.";
    }

    public override void GenerateDescription() {
        Retaliationlvl[] AllLvls = GetComponents<Retaliationlvl>();
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

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        Retaliationlvl RL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                RL = GetComponent<Retaliation1>();
                break;
            case 2:
                RL = GetComponent<Retaliation2>();
                break;
            case 3:
                RL = GetComponent<Retaliation3>();
                break;
            case 4:
                RL = GetComponent<Retaliation4>();
                break;
            case 5:
                RL = GetComponent<Retaliation5>();
                break;
        }
        TriggerChance = RL.TriggerChance;
        Reflected_DMG_Percentage = RL.Reflected_DMG_Percentage;

        GenerateDescription();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_DMG_TAKEN += RetaliationPassive;
    }

    private void RetaliationPassive(Damage dmg) {
        if (dmg.Source != null && dmg.GetType()==typeof(DirectDamage) && dmg.Type!=typeof(RetaliationDebuff)) {
            if (UnityEngine.Random.value < (TriggerChance / 100)) {
                float reflected_dmg_amount = dmg.Amount * (Reflected_DMG_Percentage / 100);//No trace back
                ApplyRetaliationDebuff(dmg.Source,reflected_dmg_amount,dmg.Crit);
            }
        }
    }

    private void ApplyRetaliationDebuff(ObjectController target,float reflected_dmg_amount,bool IsCrit) {
        RetaliationDebuff RD = RetaliationDebuff.Generate(reflected_dmg_amount, IsCrit);
        RD.ApplyDebuff(OC,target);
    }
}
