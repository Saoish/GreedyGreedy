using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class BloodForBlood : PassiveSkill {

    float LPH_INC_Perentage;
    public float HealthTriggerThreshold = 30;
    public float TriggerCD;
    public float Duration;

    private float RealTime_TriggerCD = 0;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        BloodForBloodlvl BFBL = null;
        switch (this.SD.lvl) {
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
        Description = "Increase your life steal by " + LPH_INC_Perentage + "% when your health fall below "+ HealthTriggerThreshold+"%. Effect lasts "+ Duration+" secs and can not be triggered again within "+ TriggerCD+" secs.";
    }

    protected override void Update() {
        base.Update();
        if (RealTime_TriggerCD > 0)
            RealTime_TriggerCD -= Time.deltaTime;
        else
            ResetRealTimeTriggerCD();
    }

    public override void ApplyPassive() {
        OC.ON_HEALTH_UPDATE += BFBPassive;
    }

    private void BFBPassive(Value health_mod) {
        if (health_mod.Type == 0) {//Damage type
            if ((OC.GetCurrStats(StatsType.HEALTH) - health_mod.Amount) / OC.GetMaxStats(StatsType.HEALTH) <= HealthTriggerThreshold / 100) {
                if (RealTime_TriggerCD == 0 && !OC.HasBuff(typeof(BloodForBloodBuff))) {
                    ApplyBloodForBloodBuff();
                    RealTime_TriggerCD = TriggerCD;
                }
            }
        }
    }


    private void ResetRealTimeTriggerCD() {
        RealTime_TriggerCD = 0;
    }

    private void ApplyBloodForBloodBuff() {
        BloodForBloodBuff BFB_Buff = BloodForBloodBuff.Generate(LPH_INC_Perentage, Duration);
        BFB_Buff.ApplyBuff(OC);
    }
}
