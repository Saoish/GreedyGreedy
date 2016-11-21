using UnityEngine;
using System.Collections;
using System;

public class HealingBuff : Buff {

    public AudioClip heal_sfx;

    public float HealingInterval = 1f;

    private float HealingTimer = 0f;

    Value heal;
	protected override void Update () {
        base.Update();
        HealPerSecond();
	}

    public override void ApplyBuff(ModData MD, ObjectController target) {
        base.ApplyBuff(MD, target);
        ModAmount = MD.ModHealth;
        heal = Value.CreateValue(ModAmount,1);
        Duration = MD.Duration;
        target.ActiveVFXParticle("HealingBuffVFX");
    }

    protected override void RemoveBuff() {
        target.DeactiveVFXParticle("HealingBuffVFX");
        DestroyObject(gameObject);
    }

    private void Heal(Value heal) {
        target.ON_HEALTH_UPDATE += target.HealHP;
        target.ON_HEALTH_UPDATE(heal);
        target.ON_HEALTH_UPDATE -= target.HealHP;
        AudioSource.PlayClipAtPoint(heal_sfx, transform.position, GameManager.SFX_Volume);
    }

    private void HealPerSecond() {
        if (HealingTimer < HealingInterval) {
            HealingTimer += Time.deltaTime;
        }
        else if(HealingTimer >= HealingInterval) {
            Heal(heal);
            HealingTimer = 0;
        }
    }
}
