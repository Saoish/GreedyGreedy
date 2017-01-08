using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;
public class IronWeaponMastery : PassiveSkill {
    float DEF_INC_Percentage;

    float DEF_Amount_INC;

    string DescriptionTemplate(IronWeaponMasterylvl[] AllLvls, int Index) {
        return "\nIncrease your defensive by " + MyText.Colofied(AllLvls[Index].DEF_INC_Percentage+ "%", highlight) + " when you have a shield equipped.";
    }

    public override void GenerateDescription() {
        IronWeaponMasterylvl[] AllLvls = GetComponents<IronWeaponMasterylvl>();
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

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        IronWeaponMasterylvl IL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                IL = GetComponent<IronWeaponMastery1>();
                break;
            case 2:
                IL = GetComponent<IronWeaponMastery2>();
                break;
            case 3:
                IL = GetComponent<IronWeaponMastery3>();
                break;
            case 4:
                IL = GetComponent<IronWeaponMastery4>();
                break;
            case 5:
                IL = GetComponent<IronWeaponMastery5>();
                break;
        }
        DEF_INC_Percentage = IL.DEF_INC_Percentage;

        GenerateDescription();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        WeaponController WC = ((Player)OC).GetWC();
        if (!WC) {
            return;
        }
        if (WC.Type == WEAPONTYPE.SwordShield) {
            OC.AddMaxStats(STATSTYPE.DEFENSE, (float)System.Math.Round(OC.GetMaxStats(STATSTYPE.DEFENSE) * (DEF_INC_Percentage / 100),1));
        }
    }
}
