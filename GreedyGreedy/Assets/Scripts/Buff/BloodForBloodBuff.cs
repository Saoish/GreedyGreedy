using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class BloodForBloodBuff : Buff {
    public GameObject VFX;

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

    public override void ApplyBuff(ObjectController applyer, ObjectController target) {
        base.ApplyBuff(applyer, target);
        ModAmount = (float)System.Math.Round(target.GetMaxStats(STATSTYPE.LPH) * (LPH_INC_Percentage / 100),1);
        target.AddCurrStats(STATSTYPE.LPH, ModAmount);
        target.ActiveVFXParticle(VFX);
    }

    public override void RemoveBuff() {
        target.DecCurrStats(STATSTYPE.LPH, ModAmount);
        target.DeactiveVFXParticle(VFX);
        DestroyObject(gameObject);
    }

}
