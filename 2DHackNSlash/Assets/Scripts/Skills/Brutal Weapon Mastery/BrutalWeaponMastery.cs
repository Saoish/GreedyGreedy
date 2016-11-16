using UnityEngine;
using System.Collections;

public class BrutalWeaponMastery : PassiveSkill{
    float AD_INC_Percentage;

    float AD_Amount_INC;

    protected override void Awake(){
        base.Awake();
    }

    public override void InitSkill(int lvl){
        base.InitSkill(lvl);
        BrutalWeaponMasterylvl BL = null;
        switch (this.SD.lvl)
        {
            case 0:
                return;
            case 1:
                BL = GetComponent<BrutalWeaponMastery1>();
                break;
            case 2:
                BL = GetComponent<BrutalWeaponMastery2>();
                break;
            case 3:
                BL = GetComponent<BrutalWeaponMastery3>();
                break;
            case 4:
                BL = GetComponent<BrutalWeaponMastery4>();
                break;
            case 5:
                BL = GetComponent<BrutalWeaponMastery5>();
                break;
        }
        AD_INC_Percentage = BL.AD_INC_Percentage;
        OC = transform.parent.parent.GetComponent<ObjectController>();
    }

    protected override void Start(){
        base.Start();
    }

    protected override void Update(){
        base.Update();
    }

    public override void ApplyPassive() {//Error will be raised if try to apply on Monsters
        WeaponController WC = ((PlayerController)OC).GetEquippedWeaponController();
        if (!WC) {
            return;
        }
        if (WC.Type == 0 || WC.Type == 1) {
            OC.SetMaxAD(OC.GetMaxAD() + OC.GetMaxAD() * (AD_INC_Percentage / 100));
        } 
    }
}