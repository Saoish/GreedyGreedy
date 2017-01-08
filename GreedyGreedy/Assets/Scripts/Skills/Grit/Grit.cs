using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class Grit : PassiveSkill {
    float HP_INC_Percentage;

    string DescriptionTemplate(Gritlvl[] AllLvls, int Index) {
        return "\nIncrease your max health by " + MyText.Colofied(AllLvls[Index].HP_INC_Percentage + "%",highlight) + ".";
    }

    public override void GenerateDescription() {
        Gritlvl[] AllLvls = GetComponents<Gritlvl>();
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
        Gritlvl GL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                GL = GetComponent<Grit1>();
                break;
            case 2:
                GL = GetComponent<Grit2>();
                break;
            case 3:
                GL = GetComponent<Grit3>();
                break;
            case 4:
                GL = GetComponent<Grit4>();
                break;
            case 5:
                GL = GetComponent<Grit5>();
                break;
        }
        HP_INC_Percentage = GL.HP_INC_Percentage;

        GenerateDescription();
    }

    public override void ApplyPassive() {
        OC.AddMaxStats(STATSTYPE.HEALTH, (float)System.Math.Round(OC.GetMaxStats(STATSTYPE.HEALTH) * (HP_INC_Percentage / 100),0));
    }
}
