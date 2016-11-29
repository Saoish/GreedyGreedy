using UnityEngine;
using System.Collections;
using System;

public class RuinDebuff : Debuff {
    public AudioClip TriggerSFX;
    void Awake() {

    }


    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void ApplyDebuff(ModData MD, ObjectController target) {
        base.ApplyDebuff(MD, target);
        ModAmount = target.GetMaxMoveSpd() * (MD.ModMoveSpd / 100);
        target.SetCurrMoveSpd(target.GetCurrMoveSpd() - ModAmount);
        Duration = MD.Duration;
        target.ActiveVFXParticle("RuinDebuffVFX", Layer.Skill);
        AudioSource.PlayClipAtPoint(TriggerSFX, target.transform.position, GameManager.SFX_Volume);
    }

    protected override void RemoveDebuff() {
        target.SetCurrMoveSpd(target.GetCurrMoveSpd() + ModAmount);
        target.DeactiveVFXParticle("RuinDebuffVFX");
        DestroyObject(gameObject);
    }
}
