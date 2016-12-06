using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class Grit : PassiveSkill {
    float HP_INC_Percentage;

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        Gritlvl GL = null;
        switch (this.SD.lvl) {
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

        Description = "Increase your max health by "+HP_INC_Percentage+"%.";
    }

    public override void ApplyPassive() {
        OC.AddMaxStats(StatsType.HEALTH, OC.GetMaxStats(StatsType.HEALTH) * (HP_INC_Percentage / 100));
    }
}
