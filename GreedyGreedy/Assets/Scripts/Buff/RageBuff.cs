using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class RageBuff : Buff {
    public GameObject VFX;

    float ModAmount;
    float Damage_INC_Percentage;

    void Awake() {

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public static RageBuff Generate(float AD_INC_Percentage, float Duration) {
        GameObject RB_OJ = Instantiate(Resources.Load("BuffPrefabs/RageBuff")) as GameObject;
        RB_OJ.name = "RageBuff";
        RB_OJ.GetComponent<RageBuff>().Duration = Duration;
        RB_OJ.GetComponent<RageBuff>().Damage_INC_Percentage = AD_INC_Percentage;
        return RB_OJ.GetComponent<RageBuff>();
    }

    public override void ApplyBuff(ObjectController applyer, ObjectController target) {
        base.ApplyBuff(applyer, target);
        ModAmount = (float)System.Math.Round(target.GetMaxStats(STATSTYPE.DAMAGE) * (Damage_INC_Percentage / 100),0);
        target.AddCurrStats(STATSTYPE.DAMAGE,ModAmount);
        target.ActiveVFXParticle(VFX);
    }

    public override void RemoveBuff() {
        target.DecCurrStats(STATSTYPE.DAMAGE, ModAmount);
        target.DeactiveVFXParticle(VFX);
        Destroy(gameObject);
    }
}
