using UnityEngine;
using System.Collections;
using GreedyNameSpace;
using System;

public abstract class ActiveSkill : Skill {
    public enum Type {
        Instant,
        Cast,
        Manual
    }

    [HideInInspector]
    public bool Indicating = false;
    [HideInInspector]
    public float CD;
    [HideInInspector]
    public float RealTime_CD = 0;
    [HideInInspector]
    public float EssenseCost = 0;

    public Type type;

    protected override void Awake() {
        base.Awake();
    }

    public override void Delete() {        
        Destroy(gameObject);
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);        
    }

    protected override void Start () {
        base.Start();
	}

    protected override void Update () {
        base.Update();
        if (OC!=null && OC.Stunned)
            Interrupt();
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

    public bool Ready() {
        if (OC.Stunned) {
            RedNotification.Push(RedNotification.Type.STUNNED);
            return false;
        } else if (RealTime_CD > 0) {
            RedNotification.Push(RedNotification.Type.ON_CD);
            return false;
        } else if (OC.GetCurrStats(STATSTYPE.ESSENSE) - EssenseCost < 0) {
            RedNotification.Push(RedNotification.Type.NO_MANA);
            return false;
        }
        return true;
    }

    public abstract void Active();

    //Non-Instant skill
    public virtual void ActiveIndicator() {}

    public virtual void Interrupt() {}
}
