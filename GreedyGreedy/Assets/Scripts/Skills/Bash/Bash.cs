using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class Bash : PassiveSkill {
    float TriggerChance;

    public float StunDuration = 0.5f;

    string DescriptionTemplate(Bashlvl[] AllLvls, int Index) {
        return "\nUpon dealing damage, you have " + MyText.Colofied(AllLvls[Index].TriggerChance + "%", highlight) + " chance to stun your targets for " + StunDuration + " secs.";
    }

    public override void GenerateDescription() {
        Bashlvl[] AllLvls = GetComponents<Bashlvl>();
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
        Bashlvl BL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                BL = GetComponent<Bash1>();
                break;
            case 2:
                BL = GetComponent<Bash2>();
                break;
            case 3:
                BL = GetComponent<Bash3>();
                break;
            case 4:
                BL = GetComponent<Bash4>();
                break;
            case 5:
                BL = GetComponent<Bash5>();
                break;
        }
        TriggerChance = BL.TriggerChance;
        GenerateDescription();        
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_DMG_DEAL += BashPassive;
    }















    //Private
    void BashPassive(ObjectController target) {
        if (UnityEngine.Random.value < (TriggerChance / 100)) {
            if (target.HasDebuff(typeof(StunDebuff))) {
                Debuff ExistedStunDebuff = target.GetDebuff(typeof(StunDebuff));
                if (StunDuration > ExistedStunDebuff.Duration)
                    ExistedStunDebuff.Duration = StunDuration;
            } else {
                ApplyStunDebuff(target);
            }
        }
    }


    private void ApplyStunDebuff(ObjectController target) {
        StunDebuff SD = StunDebuff.Generate(StunDuration);
        SD.ApplyDebuff(OC,target);
    }
}
