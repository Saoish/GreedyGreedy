using UnityEngine;
using System.Collections;

public class RetaliationDebuff : Debuff {
    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void ApplyDebuff(ModData MD, ObjectController target,Value debuff_dmg) {
        base.ApplyDebuff(MD, target,debuff_dmg);
        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(debuff_dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
        target.ActiveOneShotVFXParticle("RetaliationDebuffVFX");
    }

    protected override void RemoveDebuff() {
        DestroyObject(gameObject);
    }
}


