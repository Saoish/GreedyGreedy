using UnityEngine;
using System.Collections;

public abstract class ActiveSkill : Skill {
    [HideInInspector]
    public float CD;

    [HideInInspector]
    public float RealTime_CD = 0;
    [HideInInspector]
    public float ManaCost = 0;

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
    }

    protected override void Start () {
        base.Start();
	}

    protected override void Update () {
        base.Update();
        if (RealTime_CD > 0)
            RealTime_CD -= Time.deltaTime;
        else
            ResetCD();
	}

    public virtual void ResetCD() {
        RealTime_CD = 0;
    }

    public float GetCDPortion() {
        return RealTime_CD / CD;
    }

    public abstract bool Ready();
    
    public abstract void Active();
}
