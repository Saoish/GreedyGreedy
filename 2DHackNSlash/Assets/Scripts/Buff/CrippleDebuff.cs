using UnityEngine;
using System.Collections;
using System;

public class CrippleDebuff : Debuff {

    private float ModADAmount;
    private float ModMDAmount;

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}


    public override void ApplyDebuff(ModData MD, ObjectController target) {
        base.ApplyDebuff(MD, target);
        ModADAmount = target.GetMaxAD() * (MD.ModAD / 100);
        ModMDAmount = target.GetMaxMD() * (MD.ModMD / 100);
        target.SetCurrAD(target.GetCurrAD() - ModADAmount);
        target.SetCurrMD(target.GetCurrMD() - ModMDAmount);
        Duration = MD.Duration;
        target.ActiveVFXParticle("CrippleDebuffVFX", Layer.Skill);
    }

    protected override void RemoveDebuff() {
        target.SetCurrAD(target.GetCurrAD() + ModADAmount);
        target.SetCurrMD(target.GetCurrMD() + ModMDAmount);
        target.DeactiveVFXParticle("CrippleDebuffVFX");
        Destroy(gameObject);
    }
}
