using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class RageBuff : Buff {
    float ModAmount;
    float AD_INC_Percentage;

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
        RB_OJ.GetComponent<RageBuff>().AD_INC_Percentage = AD_INC_Percentage;
        return RB_OJ.GetComponent<RageBuff>();
    }

    public override void ApplyBuff(ObjectController target) {
        base.ApplyBuff(target);
        ModAmount = target.GetMaxStats(StatsType.AD) * (AD_INC_Percentage / 100);
        target.AddCurrStats(StatsType.AD,ModAmount);
        target.ActiveVFXParticle("RageBuffVFX");
    }

    protected override void RemoveBuff() {
        target.DecCurrStats(StatsType.AD, ModAmount);
        target.DeactiveVFXParticle("RageBuffVFX");
        Destroy(gameObject);
    }
}
