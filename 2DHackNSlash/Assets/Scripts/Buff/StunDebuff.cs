using UnityEngine;
using System.Collections;
using System;

public class StunDebuff : Debuff {
    public AudioClip StunSFX;
    protected override void Update() {
        base.Update();
    }

    public override void ApplyDebuff(ModData MD, ObjectController target) {
        base.ApplyDebuff(MD, target);
        target.Stunned = true;
        Duration = MD.Duration;
        target.ActiveVFXParticle("StunDebuffVFX", Layer.Skill);
        AudioSource.PlayClipAtPoint(StunSFX, transform.position, GameManager.SFX_Volume);
    }

    protected override void RemoveDebuff() {
        target.Stunned = false;
        target.DeactiveVFXParticle("StunDebuffVFX");
        DestroyObject(gameObject);
    }

}
