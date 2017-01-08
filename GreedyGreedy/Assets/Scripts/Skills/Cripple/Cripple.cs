using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class Cripple : PassiveSkill {

    float TriggerChance;
    float DMG_DEC_Percentage;

    public float Duration = 5;

    string DescriptionTemplate(Cripplelvl[] AllLvls, int Index) {
        return "\nUpon taking dmg, you have " + MyText.Colofied(AllLvls[Index].TriggerChance+ "%",highlight) + " chance to cripple the attacker which lowers their damage by " + MyText.Colofied(AllLvls[Index].DMG_DEC_Percentage + "%", highlight) + "% for " + Duration + " secs, Cripple effect does not stack.";
    }

    public override void GenerateDescription() {
        Cripplelvl[] AllLvls = GetComponents<Cripplelvl>();
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
        Cripplelvl CL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                CL = GetComponent<Cripple1>();
                break;
            case 2:
                CL = GetComponent<Cripple2>();
                break;
            case 3:
                CL = GetComponent<Cripple3>();
                break;
            case 4:
                CL = GetComponent<Cripple4>();
                break;
            case 5:
                CL = GetComponent<Cripple5>();
                break;
        }
        TriggerChance = CL.TriggerChance;
        DMG_DEC_Percentage = CL.DMG_DEC_Percentage;

        GenerateDescription();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_DMG_TAKEN += CripplePassive;
    }








    //Private
    void CripplePassive(Damage dmg) {
        if (dmg.GetType() == typeof(DirectDamage)) {
            if (dmg.Source != null && !dmg.Source.HasDebuff(typeof(CrippleDebuff))) {
                if (UnityEngine.Random.value < (TriggerChance / 100)) {
                    ApplyCrippleDebuff(dmg.Source);
                }
            }
        }
    }

    void ApplyCrippleDebuff(ObjectController target) {
        CrippleDebuff cripple_debuff = CrippleDebuff.Generate(DMG_DEC_Percentage, Duration);
        cripple_debuff.ApplyDebuff(OC,target);
    }
}
