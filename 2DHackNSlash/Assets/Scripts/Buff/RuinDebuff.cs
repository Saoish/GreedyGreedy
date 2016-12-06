using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class RuinDebuff : Debuff {
    public AudioClip TriggerSFX;
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

    public override void ApplyDebuff(ObjectController target) {
        base.ApplyDebuff(target);
        ModAmount = target.GetMaxStats(StatsType.MOVE_SPEED) * (MOVESPD_DEC_Percentage / 100);
        target.DecCurrStats(StatsType.MOVE_SPEED,ModAmount);
        target.ActiveVFXParticle("RuinDebuffVFX");
        AudioSource.PlayClipAtPoint(TriggerSFX, target.transform.position, GameManager.SFX_Volume);
    }

    protected override void RemoveDebuff() {
        target.AddCurrStats(StatsType.MOVE_SPEED, ModAmount);
        target.DeactiveVFXParticle("RuinDebuffVFX");
        DestroyObject(gameObject);
    }
}
