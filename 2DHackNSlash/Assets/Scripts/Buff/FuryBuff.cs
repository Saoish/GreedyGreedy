using UnityEngine;
using System.Collections;

public class FuryBuff : Buff {
    void Awake() {

    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void ApplyBuff(ModData MD, ObjectController target) {
        base.ApplyBuff(MD, target);
        ModAmount = target.GetMaxAttkSpd() * (MD.ModAttSpd / 100);
        target.SetCurrAttkSpd(target.GetCurrAttkSpd() + ModAmount);
        Duration = MD.Duration;
        //target.ActiveVFXParticle("FuryBuffVFX");
    }

    protected override void RemoveBuff() {
        if (target == null)
            return;
        target.SetCurrAttkSpd(target.GetCurrAttkSpd() - ModAmount);
        //target.DeactiveVFXParticle("FuryBuffVFX");
        Destroy(gameObject);
    }
}
