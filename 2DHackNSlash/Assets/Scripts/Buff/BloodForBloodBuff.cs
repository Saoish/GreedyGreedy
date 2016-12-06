using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class BloodForBloodBuff : Buff {
    float ModAmount;
    float LPH_INC_Percentage;

    public static BloodForBloodBuff Generate(float LPH_INC_Percentage,float Duration) {
        GameObject BFBB_OJ = Instantiate(Resources.Load("BuffPrefabs/BloodForBloodBuff")) as GameObject;
        BFBB_OJ.name = "BloodForBloodBuff";
        BFBB_OJ.GetComponent<BloodForBloodBuff>().Duration = Duration;
        BFBB_OJ.GetComponent<BloodForBloodBuff>().LPH_INC_Percentage = LPH_INC_Percentage;
        return BFBB_OJ.GetComponent<BloodForBloodBuff>();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyBuff(ObjectController target) {
        base.ApplyBuff(target);
        ModAmount = target.GetMaxStats(StatsType.LPH) * (LPH_INC_Percentage / 100);
        target.AddCurrStats(StatsType.LPH, ModAmount);
        target.ActiveVFXParticle("BloodForBloodBuffVFX");
    }

    protected override void RemoveBuff() {
        target.DecCurrStats(StatsType.LPH, ModAmount);
        target.DeactiveVFXParticle("BloodForBloodBuffVFX");
        DestroyObject(gameObject);
    }

}
