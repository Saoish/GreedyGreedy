using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class BleedDebuff : Debuff {
    public GameObject VFX;
    public AudioClip SFX;

    public float BleedingInterval = 1f;

    private float BleedingTimer = 0f;

    float BleedAmount;

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        BleedPerSecond();
    }

    public static BleedDebuff Generate(float BleedAmount, float Duration) {
        GameObject BD_OJ = Instantiate(Resources.Load("DebuffPrefabs/BleedDebuff")) as GameObject;
        BD_OJ.name = "BleedDebuff";
        BD_OJ.GetComponent<BleedDebuff>().Duration = Duration;
        BD_OJ.GetComponent<BleedDebuff>().BleedAmount = BleedAmount;
        return BD_OJ.GetComponent<BleedDebuff>();
    }

    public override void ApplyDebuff(ObjectController applyer, ObjectController target) {
        base.ApplyDebuff(applyer,target);
        target.ActiveVFXParticle(VFX);
        DealBleedDmg();
    }

    public override void RemoveDebuff() {
        target.DeactiveVFXParticle(VFX);
        DestroyObject(gameObject);
    }

    private void BleedPerSecond() {
        if (BleedingTimer < BleedingInterval) {
            BleedingTimer += Time.deltaTime;
        }
        else if (BleedingTimer >= BleedingInterval) {
            DealBleedDmg();
            BleedingTimer = 0;
        }
    }


    private void DealBleedDmg() {
        float RawDamage = BleedAmount;
        bool Crit = false;
        if(applyer != null && UnityEngine.Random.value < (applyer.GetCurrStats(STATSTYPE.CRIT_CHANCE) / 100))
            RawDamage = RawDamage * (applyer.GetCurrStats(STATSTYPE.CRIT_DMG) / 100);
        DotDamage BleedDamage = new DotDamage(RawDamage, Crit, applyer, typeof(BleedDebuff));
        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(BleedDamage);
        target.ON_DMG_TAKEN -= target.DeductHealth;
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }
}
