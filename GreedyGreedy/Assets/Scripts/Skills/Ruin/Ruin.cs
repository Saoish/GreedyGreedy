using UnityEngine;
using System.Collections;
using GreedyNameSpace;
public class Ruin : PassiveSkill {
    [HideInInspector]
    public float TriggerChance;
    [HideInInspector]
    public float MOVESPD_DEC_Percentage;

    public float Duration = 5;

    string DescriptionTemplate(Ruinlvl[] AllLvls, int Index) {
        return "\nUpon dealing damage, you have " + MyText.Colofied(AllLvls[Index].TriggerChance+ "%",highlight) + " chance to slow down enemy movement speed by " + MyText.Colofied(AllLvls[Index].MOVESPD_DEC_Percentage + "%", highlight) + " for "+ Duration+" secs.";
    }

    public override void GenerateDescription() {
        Ruinlvl[] AllLvls = GetComponents<Ruinlvl>();
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
        Ruinlvl RL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                RL = GetComponent<Ruin1>();
                break;
            case 2:
                RL = GetComponent<Ruin2>();
                break;
            case 3:
                RL = GetComponent<Ruin3>();
                break;
            case 4:
                RL = GetComponent<Ruin4>();
                break;
            case 5:
                RL = GetComponent<Ruin5>();
                break;
        }
        TriggerChance = RL.TriggerChance;
        MOVESPD_DEC_Percentage = RL.MOVESPD_DEC_Percentage;
        GenerateDescription();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_DMG_DEAL += RuinPassive;
    }












    //Private
    void RuinPassive(ObjectController target) {
        if (UnityEngine.Random.value < (TriggerChance / 100)) {
            if (!target.HasDebuff(typeof(RuinDebuff))) {
                ApplyRuinDebuff(target);
            }
        }
    }

    void ApplyRuinDebuff(ObjectController target) {
        RuinDebuff RD = RuinDebuff.Generate(MOVESPD_DEC_Percentage,Duration);
        RD.ApplyDebuff(OC,target);
    }
}
