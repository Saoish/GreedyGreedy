using UnityEngine;
using System.Collections;
using System;

public class RageBuff : Buff {

    [HideInInspector]
    public float ModAmount;

    void Awake() {

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public override void ApplyBuff(ModData MD, ObjectController target) {
        base.ApplyBuff(MD, target);
        ModAmount = target.GetMaxAD() * (MD.ModAD / 100);
        target.SetCurrAD(target.GetCurrAD() + ModAmount);
        Duration = MD.Duration;
        target.ActiveVFXParticle("RageBuffVFX");
    }

    protected override void RemoveBuff() {
        if (target == null)
            return;
        target.SetCurrAD(target.GetCurrAD() - ModAmount);
        target.DeactiveVFXParticle("RageBuffVFX");
        Destroy(gameObject);
    }
}
