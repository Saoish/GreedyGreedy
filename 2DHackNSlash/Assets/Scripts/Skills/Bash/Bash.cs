using UnityEngine;
using System.Collections;
using System;

public class Bash : PassiveSkill {
    float TriggerChance;

    public float StunDuration = 1f;

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        Bashlvl BL = null;
        switch (this.SD.lvl) {
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
        ModData StunDebuffMod = ScriptableObject.CreateInstance<ModData>();
        StunDebuffMod.Name = "StunDebuff";
        StunDebuffMod.Duration = StunDuration;
        GameObject StunDebuffObject = Instantiate(Resources.Load("DebuffPrefabs/" + StunDebuffMod.Name)) as GameObject;
        StunDebuffObject.name = StunDebuffMod.Name;
        StunDebuffObject.GetComponent<Debuff>().ApplyDebuff(StunDebuffMod, target);

    }
}
