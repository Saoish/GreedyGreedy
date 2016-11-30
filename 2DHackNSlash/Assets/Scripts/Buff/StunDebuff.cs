using UnityEngine;
using System.Collections;
using System;

public class StunDebuff : Debuff {
    protected override void Update() {
        base.Update();
    }

    public override void ApplyDebuff(ModData MD, ObjectController target) {
        base.ApplyDebuff(MD, target);
        target.Stunned = true;
        //target.MountainlizeRigibody();
        Duration = MD.Duration;
        target.ActiveVFXParticle("StunDebuffVFX", Layer.Skill);
    }

    protected override void RemoveDebuff() {
        target.Stunned = false;
        //target.NormalizeRigibody();
        target.DeactiveVFXParticle("StunDebuffVFX");
        DestroyObject(gameObject);
    }

}
