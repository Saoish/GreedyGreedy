using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class Vigor : PassiveSkill {

    float AD_INC_Percentage;

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
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

        Description = "Increase your Attack Damage by " + AD_INC_Percentage + "%.";
    }

    public override void ApplyPassive() {
        float ad_inc_value = OC.GetMaxStats(StatsType.AD) * (AD_INC_Percentage / 100);
        OC.AddMaxStats(StatsType.AD, ad_inc_value);
        //OC.SetMaxAD(OC.GetMaxAD() + OC.GetMaxAD() * (AD_INC_Percentage / 100));
    }

}
