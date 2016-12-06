using UnityEngine;
using System.Collections;
using System;

public class BleedDebuff : Debuff {

    public AudioClip bleed_sfx;

    public float BleedingInterval = 1f;

    private float BleedingTimer = 0f;

    Value BleedValue;
    // Update is called once per frame
    protected override void Update() {
        base.Update();
        BleedPerSecond();
    }

    public static BleedDebuff Generate(float BleedAmount, float Duration) {
        GameObject BD_OJ = Instantiate(Resources.Load("DebuffPrefabs/BleedDebuff")) as GameObject;
        BD_OJ.name = "BleedDebuff";
        BD_OJ.GetComponent<BleedDebuff>().Duration = Duration;
        BD_OJ.GetComponent<BleedDebuff>().BleedValue = new Value(BleedAmount, 0, false,null,false, true);
        return BD_OJ.GetComponent<BleedDebuff>();
    }

    public override void ApplyDebuff(ObjectController target) {
        base.ApplyDebuff(target);
        target.ActiveVFXParticle("BleedDebuffVFX");
        DealBleedDmg(BleedValue);
    }

    protected override void RemoveDebuff() {
        target.DeactiveVFXParticle("BleedDebuffVFX");
        DestroyObject(gameObject);
    }

    private void BleedPerSecond() {
        if (BleedingTimer < BleedingInterval) {
            BleedingTimer += Time.deltaTime;
        }
        else if (BleedingTimer >= BleedingInterval) {
            DealBleedDmg(BleedValue);
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
