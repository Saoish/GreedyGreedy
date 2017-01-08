using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class Vigor : PassiveSkill {

    float Damge_INC_Percentage;

    string DescriptionTemplate(Vigorlvl[] AllLvls, int Index) {
        return "\nIncrease your damage by " + MyText.Colofied(AllLvls[Index].Damge_INC_Percentage+"%",highlight) + ".";
    }

    public override void GenerateDescription() {
        Vigorlvl[] AllLvls = GetComponents<Vigorlvl>();
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
        Vigorlvl VL = null;
        switch (this.lvl) {
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
        Damge_INC_Percentage = VL.Damge_INC_Percentage;

        GenerateDescription();
    }

    public override void ApplyPassive() {        
        float damage_inc_value =(float)System.Math.Round(OC.GetMaxStats(STATSTYPE.DAMAGE) * (Damge_INC_Percentage / 100),0);
        OC.AddMaxStats(STATSTYPE.DAMAGE, damage_inc_value);
        //OC.SetMaxAD(OC.GetMaxAD() + OC.GetMaxAD() * (AD_INC_Percentage / 100));
    }

}
