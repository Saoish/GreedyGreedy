using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;
public class IronWeaponMastery : PassiveSkill {
    float DEF_INC_Percentage;

    float DEF_Amount_INC;

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        IronWeaponMasterylvl IL = null;
        switch (this.SD.lvl) {
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

        Description = "Increase your max defensive by "+DEF_INC_Percentage+"% when you have a shield equipped.";
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        WeaponController WC = ((PlayerController)OC).GetWC();
        if (!WC) {
            return;
        }
        if (WC.Type == WeaponType.SwordShield) {
            OC.AddMaxStats(StatsType.DEFENSE, OC.GetMaxStats(StatsType.DEFENSE) * (DEF_INC_Percentage / 100));
        }
    }
}
