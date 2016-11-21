using UnityEngine;
using System.Collections;

public class Cripple : PassiveSkill {

    float TriggerChance;
    float DMG_DEC_Percentage;

    public float Duration = 5;


    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        Cripplelvl CL = null;
        switch (this.SD.lvl) {
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
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void ApplyPassive() {
        OC.ON_HEALTH_UPDATE += CripplePassive;
    }








    //Private
    void CripplePassive(Value dmg) {
        if (dmg.SourceOC != null && !dmg.SourceOC.HasDebuff(typeof(CrippleDebuff))) {
            if(UnityEngine.Random.value < (TriggerChance / 100)) {
                ApplyCrippleDebuff(dmg.SourceOC);
            }
        }
    }

    void ApplyCrippleDebuff(ObjectController target) {
        ModData CrippleDebuffMod = ScriptableObject.CreateInstance<ModData>();
        CrippleDebuffMod.Name = "CrippleDebuff";
        CrippleDebuffMod.ModAD = DMG_DEC_Percentage;
        CrippleDebuffMod.ModMD = DMG_DEC_Percentage;
        CrippleDebuffMod.Duration = Duration;
        GameObject CrippleDebuffObject = Instantiate(Resources.Load("DebuffPrefabs/" + CrippleDebuffMod.Name)) as GameObject;
        CrippleDebuffObject.name = "CrippleDebuff";
        CrippleDebuffObject.GetComponent<Debuff>().ApplyDebuff(CrippleDebuffMod, target);
    }
}
