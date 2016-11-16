using UnityEngine;
using System.Collections;
using System;

public class Vigor : PassiveSkill {

    float AD_INC_Percentage;
    PlayerController PC;

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        Vigorlvl VL = null;
        switch (this.SD.lvl) {
            case 0:
                return;
            case 1:
                VL = GetComponent<Vigor1>();
                break;
            case 2:
                VL = GetComponent<Vigor2>();
                break;
            case 3:
                VL = GetComponent<Vigor3>();
                break;
            case 4:
                VL = GetComponent<Vigor4>();
                break;
            case 5:
                VL = GetComponent<Vigor5>();
                break;
        }
        AD_INC_Percentage = VL.AD_INC_Perentage;
        PC = transform.parent.parent.GetComponent<PlayerController>();
    }

    public override void ApplyPassive() {
        PC.SetMaxAD(PC.GetMaxAD() + PC.GetMaxAD() * (AD_INC_Percentage / 100));
    }

}
