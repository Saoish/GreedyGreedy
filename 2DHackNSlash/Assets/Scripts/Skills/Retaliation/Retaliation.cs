using UnityEngine;
using System.Collections;

public class Retaliation : PassiveSkill {

    float TriggerChance;
    float Reflected_DMG_Percentage;

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        Retaliationlvl RL = null;
        switch (this.SD.lvl) {
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

        Description = "Upon taking damage, you have " + TriggerChance + "% chance to reflect " + Reflected_DMG_Percentage + "% taken damge to attacker as true damage.";
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_HEALTH_UPDATE += RetaliationPassive;
    }

    private void RetaliationPassive(Value dmg) {
        if (dmg.SourceOC != null) {
            if (UnityEngine.Random.value < (TriggerChance / 100)) {
                float reflected_dmg_amount = dmg.Amount * (Reflected_DMG_Percentage / 100);//No trace back
                ApplyRetaliationDebuff(dmg.SourceOC,reflected_dmg_amount,dmg.IsCrit);
            }
        }
    }

    private void ApplyRetaliationDebuff(ObjectController target,float reflected_dmg_amount,bool IsCrit) {
        RetaliationDebuff RD = RetaliationDebuff.Generate(reflected_dmg_amount, IsCrit);
        RD.ApplyDebuff(target);
    }
}
