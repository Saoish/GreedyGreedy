﻿using UnityEngine;
using System.Collections;
using System;

public class BleedDebuff : Debuff {

    public AudioClip bleed_sfx;

    public float BleedingInterval = 1f;

    private float BleedingTimer = 0f;

    Value bleed_dmg;
    // Update is called once per frame
    protected override void Update() {
        base.Update();
        BleedPerSecond();
    }

    public override void ApplyDebuff(ModData MD, ObjectController target) {
        base.ApplyDebuff(MD, target);
        ModAmount = MD.ModHealth;
        bleed_dmg = Value.CreateValue(ModAmount);
        Duration = MD.Duration;
        target.ActiveVFXParticle("BleedDebuffVFX");
    }

    protected override void RemoveDebuff() {
        if (target == null)
            return;
        target.DeactiveVFXParticle("BleedDebuffVFX");
        DestroyObject(gameObject);
    }

    private void BleedPerSecond() {
        if (BleedingTimer < BleedingInterval) {
            BleedingTimer += Time.deltaTime;
        }
        else if (BleedingTimer >= BleedingInterval) {
            DealBleedDmg(bleed_dmg);
            BleedingTimer = 0;
        }
    }


    private void DealBleedDmg(Value bleed_dmg) {
        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(bleed_dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
        AudioSource.PlayClipAtPoint(bleed_sfx, transform.position, GameManager.SFX_Volume);
    }
}