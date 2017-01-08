using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class CrippleDebuff : Debuff {
    public GameObject VFX;
    public AudioClip SFX;
    private float ModAmount;

    float DMG_DEC_Percentage;

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public static CrippleDebuff Generate(float DMG_DEC_Percentage,float Duration) {
        GameObject CD_OJ = Instantiate(Resources.Load("DebuffPrefabs/CrippleDebuff")) as GameObject;
        CD_OJ.name = "CrippleDebuff";
        CD_OJ.GetComponent<CrippleDebuff>().Duration = Duration;
        CD_OJ.GetComponent<CrippleDebuff>().DMG_DEC_Percentage = DMG_DEC_Percentage;
        return CD_OJ.GetComponent<CrippleDebuff>();
    }

    public override void ApplyDebuff(ObjectController applyer, ObjectController target) {
        base.ApplyDebuff(applyer, target);
        ModAmount = (float)System.Math.Round(target.GetMaxStats(STATSTYPE.DAMAGE) * (DMG_DEC_Percentage / 100),1);
        target.DecCurrStats(STATSTYPE.DAMAGE,ModAmount);
        target.ActiveVFXParticle(VFX);
        AudioSource.PlayClipAtPoint(SFX, target.transform.position, GameManager.SFX_Volume);
    }

    public override void RemoveDebuff() {
        target.AddCurrStats(STATSTYPE.DAMAGE, ModAmount);
        target.DeactiveVFXParticle(VFX);
        Destroy(gameObject);
    }
}
