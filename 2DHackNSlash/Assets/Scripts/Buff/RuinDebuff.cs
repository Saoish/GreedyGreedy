using UnityEngine;
using System.Collections;
using System;

public class RuinDebuff : Debuff {
    void Awake() {

    }

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void ApplyDebuff(ModData MD, ObjectController target) {
        base.ApplyDebuff(MD, target);
        ModAmount = target.GetMaxMoveSpd() * (MD.ModMoveSpd / 100);
        target.SetCurrMoveSpd(target.GetCurrMoveSpd() - ModAmount);
        Duration = MD.Duration;
        target.ActiveVFXParticle("RuinDebuffVFX");
    }

    protected override void RemoveDebuff() {
        if (target == null)//Probably dead already
            return;
        target.SetCurrMoveSpd(target.GetCurrMoveSpd() + ModAmount);
        target.DeactiveVFXParticle("RuinDebuffVFX");
        DestroyObject(gameObject);
    }
}
