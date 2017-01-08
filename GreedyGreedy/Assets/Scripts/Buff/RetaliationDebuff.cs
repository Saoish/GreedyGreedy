using UnityEngine;
using System.Collections;

public class RetaliationDebuff : Debuff {
    public GameObject VFX;
    public AudioClip SFX;

    float ReflectedAmount;
    bool Crit;
    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }


    public static RetaliationDebuff Generate(float ReflectedAmount,bool Crit) {
        GameObject RD_OJ = Instantiate(Resources.Load("DebuffPrefabs/RetaliationDebuff")) as GameObject;
        RD_OJ.name = "RetaliationDebuff";
        RD_OJ.GetComponent<RetaliationDebuff>().ReflectedAmount = ReflectedAmount;
        RD_OJ.GetComponent<RetaliationDebuff>().Crit = Crit;
        return RD_OJ.GetComponent<RetaliationDebuff>();
    }

    public override void ApplyDebuff(ObjectController applyer, ObjectController target) {
        base.ApplyDebuff(applyer, target);
        DirectDamage damage = new DirectDamage(ReflectedAmount, 0,0, Crit, applyer, typeof(RetaliationDebuff));
        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(damage);
        target.ON_DMG_TAKEN -= target.DeductHealth;
        target.ActiveOneShotVFXParticle(VFX);
        AudioSource.PlayClipAtPoint(SFX, target.transform.position, GameManager.SFX_Volume);
    }

    public override void RemoveDebuff() {
        DestroyObject(gameObject);
    }
}


