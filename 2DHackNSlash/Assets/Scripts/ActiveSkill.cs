using UnityEngine;
using System.Collections;
using GreedyNameSpace;

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

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        gameObject.name = SD.Name;
        transform.position = OC.T_Actives.position;
        transform.SetParent(OC.T_Actives);
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

    public bool Ready() {
        if (OC.Stunned) {
            RedNotification.Push(RedNotification.Type.STUNNED);
            return false;
        } else if (RealTime_CD > 0) {
            RedNotification.Push(RedNotification.Type.ON_CD);
            return false;
        } else if (OC.GetCurrStats(StatsType.MANA) - ManaCost < 0) {
            RedNotification.Push(RedNotification.Type.NO_MANA);
            return false;
        }
        return true;
    }

    public abstract void Active();
}
