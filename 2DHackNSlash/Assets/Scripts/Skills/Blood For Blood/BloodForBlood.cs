using UnityEngine;
using System.Collections;

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

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
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
        OC = transform.parent.parent.GetComponent<ObjectController>();
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
            if ((OC.GetCurrHealth() - health_mod.Amount) / OC.GetMaxHealth() <= HealthTriggerThreshold / 100) {
                if (RealTime_TriggerCD == 0 && !OC.HasBuff(typeof(BloodForBloodBuff))) {
                    ModData BFB_BuffMod = ScriptableObject.CreateInstance<ModData>();
                    BFB_BuffMod.Name = "BloodForBloodBuff";
                    BFB_BuffMod.Duration = Duration;
                    BFB_BuffMod.ModAD = LPH_INC_Perentage;
                    GameObject BFB_Buff = Instantiate(Resources.Load("BuffPrefabs/" + BFB_BuffMod.Name)) as GameObject;
                    BFB_Buff.GetComponent<Buff>().ApplyBuff(BFB_BuffMod, OC);
                    RealTime_TriggerCD = TriggerCD;
                }
            }
        }
    }


    private void ResetRealTimeTriggerCD() {
        RealTime_TriggerCD = 0;
    }
}
