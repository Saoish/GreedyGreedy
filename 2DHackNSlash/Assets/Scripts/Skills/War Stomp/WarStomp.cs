using UnityEngine;
using System.Collections;
using System;

public class WarStomp : ActiveSkill {

    protected override void Awake() {
        base.Awake();

    }

    protected override void Start() {
        base.Start();
    }


    protected override void Update() {
        base.Update();

    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
    }

    public override bool Ready() {
        if (OC.Stunned) {
            Debug.Log(SD.Name + " " + SD.lvl + ": You are Stunned");
            return false;
        } else if (RealTime_CD > 0) {
            Debug.Log(SD.Name + " " + SD.lvl + ": Is on cooldown");
            return false;
        } else if (OC.GetCurrMana() - ManaCost < 0) {
            Debug.Log(SD.Name + " " + SD.lvl + ": Not enough mana");
            return false;
        }
        return true;
    }

    public override void Active() {
        throw new NotImplementedException();
    }

}
