using UnityEngine;
using System.Collections;

public class RetaliationDebuff : Debuff {
    Value ReflectedDmg;
    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }


    public static RetaliationDebuff Generate(float ReflectedAmount,bool IsCrit) {
        GameObject RD_OJ = Instantiate(Resources.Load("DebuffPrefabs/RetaliationDebuff")) as GameObject;
        RD_OJ.name = "RetaliationDebuff";
        RD_OJ.GetComponent<RetaliationDebuff>().ReflectedDmg = new Value(ReflectedAmount, 0, IsCrit, null);//No trace back
        return RD_OJ.GetComponent<RetaliationDebuff>();
    }

    public override void ApplyDebuff(ObjectController target) {
        base.ApplyDebuff(target);
        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(ReflectedDmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
        target.ActiveOneShotVFXParticle("RetaliationDebuffVFX");
    }

    protected override void RemoveDebuff() {
        DestroyObject(gameObject);
    }
}


