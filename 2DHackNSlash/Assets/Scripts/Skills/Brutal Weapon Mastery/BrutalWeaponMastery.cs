using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class BrutalWeaponMastery : PassiveSkill{
    float MANACOST_DEC_Percentage;

    protected override void Awake(){
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        BrutalWeaponMasterylvl BL = null;
        switch (this.SD.lvl){
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
        MANACOST_DEC_Percentage = BL.MANACOST_DEC_Percentage;
        Description = "Reduce mana cost on your auto attack by "+ MANACOST_DEC_Percentage+"% if you have Two-Handed Warrior weapon equipped.";
    }

    protected override void Start(){
        base.Start();
    }

    protected override void Update(){
        base.Update();
    }

    public override void ApplyPassive() {//Error will be raised if try to apply on Monsters
        WeaponController WC = ((PlayerController)OC).GetWC();
        if (!WC) {
            return;
        }
        if (WC.Type == WeaponType.Axe || WC.Type == WeaponType.GreatSword) {
            WC.ManaCost -= WC.ManaCost*(MANACOST_DEC_Percentage/100);
        } 
    }
}