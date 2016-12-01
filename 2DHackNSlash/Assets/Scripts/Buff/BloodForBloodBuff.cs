using UnityEngine;
using System.Collections;
using System;

public class BloodForBloodBuff : Buff {
    [HideInInspector]
    public float ModAmount;


    void Awake() {

    }

    void Start() {

    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyBuff(ModData MD, ObjectController target) {
        base.ApplyBuff(MD, target);
        ModAmount = target.GetMaxLPH() * (MD.ModAD / 100);
        target.SetCurrLPH(target.GetCurrLPH () + ModAmount);
        Duration = MD.Duration;
        target.ActiveVFXParticle("BloodForBloodBuffVFX");
    }

    protected override void RemoveBuff() {
        target.SetCurrLPH(target.GetCurrLPH() - ModAmount);
        target.DeactiveVFXParticle("BloodForBloodBuffVFX");
        DestroyObject(gameObject);
    }

}
