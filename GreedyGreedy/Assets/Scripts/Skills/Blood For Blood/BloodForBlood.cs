using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class BloodForBlood : PassiveSkill {

    float LPH_INC_Perentage;
    public float HealthTriggerThreshold = 50;
    public float TriggerCD;
    public float Duration;

    private float RealTime_TriggerCD = 0;

    string DescriptionTemplate(BloodForBloodlvl[] AllLvls, int Index) {
        return "\nIncrease your life steal by " + MyText.Colofied(AllLvls[Index].LPH_INC_Perentage+ "%",highlight) + " when your health fall below "+ HealthTriggerThreshold+" %.Effect lasts "+ Duration+" secs and can not be triggered again within "+ TriggerCD+" secs.";
    }

    public override void GenerateDescription() {
        BloodForBloodlvl[] AllLvls = GetComponents<BloodForBloodlvl>();
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
        BloodForBloodlvl BFBL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                BFBL = GetComponent<BloodForBlood1>();
                break;
            case 2:
                BFBL = GetComponent<BloodForBlood2>();
                break;
            case 3:
                BFBL = GetComponent<BloodForBlood3>();
                break;
            case 4:
                BFBL = GetComponent<BloodForBlood4>();
                break;
            case 5:
                BFBL = GetComponent<BloodForBlood5>();
                break;
        }
        LPH_INC_Perentage = BFBL.LPH_INC_Perentage;
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
        OC.ON_DMG_TAKEN += BFBPassive;
    }

    private void BFBPassive(Damage dmg) {
        if ((OC.GetCurrStats(STATSTYPE.HEALTH) - dmg.Amount) / OC.GetMaxStats(STATSTYPE.HEALTH) <= HealthTriggerThreshold / 100) {
            if (RealTime_TriggerCD == 0 && !OC.HasBuff(typeof(BloodForBloodBuff))) {
                ApplyBloodForBloodBuff();
                RealTime_TriggerCD = TriggerCD;
            }
        }        
    }


    private void ResetRealTimeTriggerCD() {
        RealTime_TriggerCD = 0;
    }

    private void ApplyBloodForBloodBuff() {
        BloodForBloodBuff BFB_Buff = BloodForBloodBuff.Generate(LPH_INC_Perentage, Duration);
        BFB_Buff.ApplyBuff(OC,OC);
    }
}
