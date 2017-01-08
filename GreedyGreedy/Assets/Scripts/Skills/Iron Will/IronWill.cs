using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;
public class IronWill : PassiveSkill {
    float DEF_INC_Percentage;

    string DescriptionTemplate(IronWilllvl[] AllLvls, int Index) {
        return "\nIncrease your defense by " + MyText.Colofied(AllLvls[Index].DEF_INC_Percentage+ "%",highlight) + ".";
    }

    public override void GenerateDescription() {
        IronWilllvl[] AllLvls = GetComponents<IronWilllvl>();
        Description = "Level: " + lvl + "/" + Patch.MaxSkilllvl;
        if (lvl == 0) {
            Description += DescriptionTemplate(AllLvls, 0);
        } else {
            Description += DescriptionTemplate(AllLvls, lvl - 1);
            if (lvl == Patch.MaxSkilllvl)
                return;
            Description += "\n\nNext Level:";
            Description += DescriptionTemplate(AllLvls, lvl);
        }
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        IronWilllvl IWL = null;
        switch (this.lvl) {
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

        GenerateDescription();
    }

    public override void ApplyPassive() {
        OC.AddMaxStats(STATSTYPE.DEFENSE, (float)System.Math.Round(OC.GetMaxStats(STATSTYPE.DEFENSE) * (DEF_INC_Percentage / 100),1));
    }
}
