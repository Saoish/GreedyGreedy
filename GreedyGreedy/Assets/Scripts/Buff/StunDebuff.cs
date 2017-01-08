using UnityEngine;
using System.Collections;
using System;

public class StunDebuff : Debuff {
    public GameObject VFX;
    protected override void Update() {
        base.Update();
    }
    
    public static StunDebuff Generate(float Duration) {
        GameObject SD_OJ = Instantiate(Resources.Load("DebuffPrefabs/StunDebuff")) as GameObject;
        SD_OJ.name = "StunDebuff";
        SD_OJ.GetComponent<StunDebuff>().Duration = Duration;
        return SD_OJ.GetComponent<StunDebuff>();
    }

    public override void ApplyDebuff(ObjectController applyer, ObjectController target) {
        base.ApplyDebuff(applyer, target);
        target.Stunned = true;
        //target.MountainlizeRigibody();
        target.ActiveVFXParticle(VFX);
    }

    public override void RemoveDebuff() {
        target.Stunned = false;
        //target.NormalizeRigibody();
        target.DeactiveVFXParticle(VFX);
        DestroyObject(gameObject);
    }

}
