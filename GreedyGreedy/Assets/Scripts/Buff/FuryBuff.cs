using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class FuryBuff : Buff {
    float ModAmount;
    float AttkSpd_INC_Percentage;
    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public static FuryBuff Generate(float AttkSpd_INC_Percentage, float Duration) {
        GameObject FB_OJ = Instantiate(Resources.Load("BuffPrefabs/FuryBuff")) as GameObject;
        FB_OJ.name = "FuryBuff";
        FB_OJ.GetComponent<FuryBuff>().Duration = Duration;
        FB_OJ.GetComponent<FuryBuff>().AttkSpd_INC_Percentage = AttkSpd_INC_Percentage;
        return FB_OJ.GetComponent<FuryBuff>();
    }

    public override void ApplyBuff(ObjectController applyer, ObjectController target) {
        base.ApplyBuff(applyer, target);
        ModAmount = (float)System.Math.Round(target.GetMaxStats(STATSTYPE.ATTACK_SPEED) * (AttkSpd_INC_Percentage / 100),1);
        target.AddCurrStats(STATSTYPE.ATTACK_SPEED,ModAmount);
    }

    public override void RemoveBuff() {
        target.DecCurrStats(STATSTYPE.ATTACK_SPEED, ModAmount);
        Destroy(gameObject);
    }
}
