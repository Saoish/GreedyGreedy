using UnityEngine;
using System.Collections;

public class Ruin : PassiveSkill {
    [HideInInspector]
    public float TriggerChance;
    [HideInInspector]
    public float MOVESPD_DEC_Percentage;

    public float Duration = 5;

    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        Ruinlvl RL = null;
        switch (this.SD.lvl) {
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
        ModData RuinDebuffMod =  ScriptableObject.CreateInstance<ModData>();
        RuinDebuffMod.Name = SD.Name+"Debuff";
        RuinDebuffMod.Duration = Duration;
        RuinDebuffMod.ModMoveSpd = MOVESPD_DEC_Percentage;
        GameObject RuinDebuffObject =  Instantiate(Resources.Load("DebuffPrefabs/" + RuinDebuffMod.Name)) as GameObject;
        RuinDebuffObject.name = SD.Name + "Debuff";
        RuinDebuffObject.GetComponent<Debuff>().ApplyDebuff(RuinDebuffMod, target);
    }
}
