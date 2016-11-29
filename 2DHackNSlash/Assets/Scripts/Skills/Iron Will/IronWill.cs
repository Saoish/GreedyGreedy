using UnityEngine;
using System.Collections;
using System;

public class IronWill : PassiveSkill {
    float DEF_INC_Percentage;

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        IronWilllvl IWL = null;
        switch (this.SD.lvl) {
            case 0:
                return;
            case 1:
                IWL = GetComponent<IronWill1>();
                break;
            case 2:
                IWL = GetComponent<IronWill2>();
                break;
            case 3:
                IWL = GetComponent<IronWill3>();
                break;
            case 4:
                IWL = GetComponent<IronWill4>();
                break;
            case 5:
                IWL = GetComponent<IronWill5>();
                break;
        }
        DEF_INC_Percentage = IWL.DEF_INC_Percentage;

        Description = "Increase your max defense by " + DEF_INC_Percentage + "%.";
    }

    public override void ApplyPassive() {
        OC.SetMaxDefense(OC.GetMaxDefense() + OC.GetMaxDefense() * (DEF_INC_Percentage / 100));
    }
}
