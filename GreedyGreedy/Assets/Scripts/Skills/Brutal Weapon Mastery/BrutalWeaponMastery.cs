using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class BrutalWeaponMastery : PassiveSkill{
    float EssenseCost_DEC_Percentage;

    string DescriptionTemplate(BrutalWeaponMasterylvl[] AllLvls, int Index) {
        return "\nReduce essense cost on your auto attack by " + MyText.Colofied(AllLvls[Index].EssenseCost_DEC_Percentage + "%",highlight) + " if you have Two-Handed Warrior weapon equipped.";
    }

    public override void GenerateDescription() {
        BrutalWeaponMasterylvl[] AllLvls = GetComponents<BrutalWeaponMasterylvl>();
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

    protected override void Awake(){
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        BrutalWeaponMasterylvl BL = null;
        switch (this.lvl){
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
        EssenseCost_DEC_Percentage = BL.EssenseCost_DEC_Percentage;
        GenerateDescription();
    }

    protected override void Start(){
        base.Start();
    }

    protected override void Update(){
        base.Update();
    }

    public override void ApplyPassive() {//Error will be raised if try to apply on Monsters
        WeaponController WC = ((Player)OC).GetWC();
        if (!WC) {
            return;
        }
        if (WC.Type == WEAPONTYPE.Axe || WC.Type == WEAPONTYPE.GreatSword) {
            WC.EssenseCost -= (float)System.Math.Round(WC.EssenseCost*(EssenseCost_DEC_Percentage/100),1);
        } 
    }
}