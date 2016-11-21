using UnityEngine;
using System.Collections;
using System;

public class Fury : ActiveSkill {
    [HideInInspector]
    public float Duration;
    [HideInInspector]
    public float AttkSpd_INC_Percentage;

    public AudioClip SFX;


    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        Furylvl FL = null;
        switch (this.SD.lvl) {
            case 0:
                return;
            case 1:
                FL = GetComponent<Fury1>();
                break;
            case 2:
                FL = GetComponent<Fury2>();
                break;
            case 3:
                FL = GetComponent<Fury3>();
                break;
            case 4:
                FL = GetComponent<Fury4>();
                break;
            case 5:
                FL = GetComponent<Fury5>();
                break;
        }
        CD = FL.CD;
        ManaCost = FL.ManaCost;
        Duration = FL.Duration;
        AttkSpd_INC_Percentage = FL.AttkSpd_INC_Percentage;
    }

    public override void Active() {
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;
        ApplyFuryBuff();
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }

    private void ApplyFuryBuff() {
        // **Note** Could possibly need to check if OC has the buff already for network
        ModData FuryBuffMod = ScriptableObject.CreateInstance<ModData>();
        FuryBuffMod.Name = "FuryBuff";
        FuryBuffMod.Duration = Duration;
        FuryBuffMod.ModAttSpd = AttkSpd_INC_Percentage;
        GameObject FuryBuffObject = Instantiate(Resources.Load("BuffPrefabs/" + FuryBuffMod.Name)) as GameObject;
        FuryBuffObject.name = "FuryBuff";
        FuryBuffObject.GetComponent<Buff>().ApplyBuff(FuryBuffMod, OC);
        RealTime_CD = CD;
    }
}
