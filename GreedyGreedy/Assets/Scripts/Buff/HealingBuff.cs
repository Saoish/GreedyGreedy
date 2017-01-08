using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class HealingBuff : Buff {

    public GameObject VFX;
    public AudioClip SFX;

    public float HealingInterval = 1f;

    private float HealingTimer = 0f;

    float HealAmount;

    float Heal_Percentage;
	protected override void Update () {
        base.Update();
        HealPerSecond();
	}

    public static HealingBuff Generate(float HealAmount,float Duration) {
        GameObject HB_OJ = Instantiate(Resources.Load("BuffPrefabs/HealingBuff")) as GameObject;
        HB_OJ.name = "HealingBuff";        
        HB_OJ.GetComponent<HealingBuff>().Duration = Duration;
        HB_OJ.GetComponent<HealingBuff>().HealAmount = HealAmount;
        return HB_OJ.GetComponent<HealingBuff>();
    }

    public override void ApplyBuff(ObjectController applyer, ObjectController target) {
        base.ApplyBuff(applyer, target);
        target.ActiveVFXParticle(VFX);
        Heal();
    }

    public override void RemoveBuff() {
        target.DeactiveVFXParticle(VFX);
        DestroyObject(gameObject);
    }

    private void Heal() {
        float RawHeal = HealAmount;
        bool Crit = false;
        if (applyer!=null && UnityEngine.Random.value < (applyer.GetCurrStats(STATSTYPE.CRIT_CHANCE) / 100))
            RawHeal = RawHeal * (applyer.GetCurrStats(STATSTYPE.CRIT_DMG) / 100);
        HealHP Heal = new HealHP(RawHeal, Crit, applyer, typeof(HealingBuff));
        target.ON_HEALTH_GAIN += target.HealHP;
        target.ON_HEALTH_GAIN(Heal);
        target.ON_HEALTH_GAIN -= target.HealHP;
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }

    private void HealPerSecond() {
        if (HealingTimer < HealingInterval) {
            HealingTimer += Time.deltaTime;
        }
        else if(HealingTimer >= HealingInterval) {
            Heal();
            HealingTimer = 0;
        }
    }
}
