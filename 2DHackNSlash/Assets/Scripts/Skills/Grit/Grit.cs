using UnityEngine;
using System.Collections;

public class Grit : PassiveSkill {
    float HP_INC_Percentage;
    
    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
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
    }

    public override void ApplyPassive() {
        OC.SetMaxHealth(OC.GetMaxHealth() + OC.GetMaxHealth() * (HP_INC_Percentage / 100));
    }
}
