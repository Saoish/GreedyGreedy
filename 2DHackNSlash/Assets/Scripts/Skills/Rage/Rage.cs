using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class Rage : PassiveSkill {

    float AD_INC_Percentage;
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
        Ragelvl RL = null;
        switch (this.SD.lvl) {
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
        AD_INC_Percentage = RL.AD_INC_Percentage;
        Description = "Increase your AD by " + AD_INC_Percentage + "% for " + Duration + " secs when your health fall below " + HealthTriggerThreshold + "%. Effect can not be triggered again within "+TriggerCD+" secs.";
    }


    protected override void Update() {
        base.Update();
        if (RealTime_TriggerCD > 0)
            RealTime_TriggerCD -= Time.deltaTime;
        else
            ResetRealTimeTriggerCD();
    }

    public override void ApplyPassive() {
        OC.ON_HEALTH_UPDATE += RagePassive;
    }

    private void RagePassive(Value health_mod) {
        if (health_mod.Type == 0) {//Damage type
            if ((OC.GetCurrStats(StatsType.HEALTH) - health_mod.Amount) / OC.GetMaxStats(StatsType.HEALTH) <= HealthTriggerThreshold / 100) {
                if (RealTime_TriggerCD == 0 && !OC.HasBuff(typeof(RageBuff))) {
                    ApplyRageBuff();
                    RealTime_TriggerCD = TriggerCD;
                }
            }
        }
    }

    private void ResetRealTimeTriggerCD() {
        RealTime_TriggerCD = 0;
    }

    private void ApplyRageBuff() {
        RageBuff RB = RageBuff.Generate(AD_INC_Percentage, Duration);
        RB.ApplyBuff(OC);
    }
}
