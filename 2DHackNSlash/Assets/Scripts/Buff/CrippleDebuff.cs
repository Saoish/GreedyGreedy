using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class CrippleDebuff : Debuff {

    private float ModADAmount;
    private float ModMDAmount;

    float DMG_DEC_Percentage;

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public static CrippleDebuff Generate(float DMG_DEC_Percentage,float Duration) {
        GameObject CD_OJ = Instantiate(Resources.Load("DebuffPrefabs/CrippleDebuff")) as GameObject;
        CD_OJ.name = "CrippleDebuff";
        CD_OJ.GetComponent<CrippleDebuff>().Duration = Duration;
        CD_OJ.GetComponent<CrippleDebuff>().DMG_DEC_Percentage = DMG_DEC_Percentage;
        return CD_OJ.GetComponent<CrippleDebuff>();
    }

    public override void ApplyDebuff(ObjectController target) {
        base.ApplyDebuff(target);
        ModADAmount = target.GetMaxStats(StatsType.AD) * (DMG_DEC_Percentage / 100);
        ModMDAmount = target.GetMaxStats(StatsType.MD) * (DMG_DEC_Percentage / 100);
        target.DecCurrStats(StatsType.AD,ModADAmount);
        target.DecCurrStats(StatsType.MD, ModMDAmount);
        target.ActiveVFXParticle("CrippleDebuffVFX");
    }

    protected override void RemoveDebuff() {
        target.AddCurrStats(StatsType.AD, ModADAmount);
        target.AddCurrStats(StatsType.MD, ModMDAmount);
        target.DeactiveVFXParticle("CrippleDebuffVFX");
        Destroy(gameObject);
    }
}
