using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class RuinDebuff : Debuff {
    public GameObject VFX;
    public AudioClip SFX;
    float ModAmount;
    float MOVESPD_DEC_Percentage;

    public static RuinDebuff Generate(float MOVESPD_DEC_Percentage,float Duration) {
        GameObject RuinDebuffObject = Instantiate(Resources.Load("DebuffPrefabs/RuinDebuff")) as GameObject;
        RuinDebuffObject.name = "RuinDebuff";
        RuinDebuffObject.GetComponent<RuinDebuff>().Duration = Duration;
        RuinDebuffObject.GetComponent<RuinDebuff>().MOVESPD_DEC_Percentage = MOVESPD_DEC_Percentage;
        return RuinDebuffObject.GetComponent<RuinDebuff>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void ApplyDebuff(ObjectController applyer, ObjectController target) {
        base.ApplyDebuff(applyer, target);
        ModAmount = (float)System.Math.Round(target.GetMaxStats(STATSTYPE.MOVE_SPEED) * (MOVESPD_DEC_Percentage / 100),1);
        target.DecCurrStats(STATSTYPE.MOVE_SPEED,ModAmount);
        target.ActiveVFXParticle(VFX);
        AudioSource.PlayClipAtPoint(SFX, target.transform.position, GameManager.SFX_Volume);
    }

    public override void RemoveDebuff() {
        target.AddCurrStats(STATSTYPE.MOVE_SPEED, ModAmount);
        target.DeactiveVFXParticle(VFX);
        DestroyObject(gameObject);
    }
}
