using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class Rage : PassiveSkill {

    float Damage_INC_Percentage;
    public float HealthTriggerThreshold = 30;
    public float TriggerCD;
    public float Duration;

    private float RealTime_TriggerCD = 0;

    string DescriptionTemplate(Ragelvl[] AllLvls, int Index) {
        return "\nIncrease your damage by " + MyText.Colofied(AllLvls[Index].Damage_INC_Percentage + "%",highlight) + " for " + Duration + " secs when your health fall below " + HealthTriggerThreshold + "%. Effect can not be triggered again within " + TriggerCD + " secs.";
    }

    public override void GenerateDescription() {
        Ragelvl[] AllLvls = GetComponents<Ragelvl>();
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

    protected override void Start() {
        base.Start();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        Ragelvl RL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                RL = GetComponent<Rage1>();
                break;
            case 2:
                RL = GetComponent<Rage2>();
                break;
            case 3:
                RL = GetComponent<Rage3>();
                break;
            case 4:
                RL = GetComponent<Rage4>();
                break;
            case 5:
                RL = GetComponent<Rage5>();
                break;
        }
        Damage_INC_Percentage = RL.Damage_INC_Percentage;
        GenerateDescription();
    }


    protected override void Update() {
        base.Update();
        if (RealTime_TriggerCD > 0)
            RealTime_TriggerCD -= Time.deltaTime;
        else
            ResetRealTimeTriggerCD();
    }

    public override void ApplyPassive() {
        OC.ON_DMG_TAKEN += RagePassive;
    }

    private void RagePassive(Damage damage) {
        if ((OC.GetCurrStats(STATSTYPE.HEALTH) - damage.Amount) / OC.GetMaxStats(STATSTYPE.HEALTH) <= HealthTriggerThreshold / 100) {
            if (RealTime_TriggerCD == 0 && !OC.HasBuff(typeof(RageBuff))) {
                ApplyRageBuff();
                RealTime_TriggerCD = TriggerCD;
            }
        }        
    }

    private void ResetRealTimeTriggerCD() {
        RealTime_TriggerCD = 0;
    }

    private void ApplyRageBuff() {
        RageBuff RB = RageBuff.Generate(Damage_INC_Percentage, Duration);
        RB.ApplyBuff(OC,OC);
    }
}
