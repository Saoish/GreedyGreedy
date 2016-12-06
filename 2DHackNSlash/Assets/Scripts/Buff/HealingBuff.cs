using UnityEngine;
using System.Collections;
using System;

public class HealingBuff : Buff {

    public AudioClip heal_sfx;

    public float HealingInterval = 1f;

    private float HealingTimer = 0f;

    Value HealValue;

    float Heal_Percentage;
	protected override void Update () {
        base.Update();
        HealPerSecond();
	}

    public static HealingBuff Generate(float HealAmount,float Duration) {
        GameObject HB_OJ = Instantiate(Resources.Load("BuffPrefabs/HealingBuff")) as GameObject;
        HB_OJ.name = "HealingBuff";
        HB_OJ.GetComponent<HealingBuff>().Duration = Duration;
        HB_OJ.GetComponent<HealingBuff>().HealValue = new Value(HealAmount,1);
        return HB_OJ.GetComponent<HealingBuff>();
    }

    public override void ApplyBuff(ObjectController target) {
        base.ApplyBuff(target);
        target.ActiveVFXParticle("HealingBuffVFX");
        Heal(HealValue);
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
            Heal(HealValue);
            HealingTimer = 0;
        }
    }
}
