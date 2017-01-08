using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;
public class Fury : ActiveSkill {
    [HideInInspector]
    public float Duration;
    [HideInInspector]
    public float AttkSpd_INC_Percentage;

    public AudioClip SFX;

    ParticleSystem FuryParticle;

    string DescriptionTemplate(Furylvl[] AllLvls, int Index) {
        return "\nBoost your attack speed by " + MyText.Colofied(AllLvls[Index].AttkSpd_INC_Percentage+ "%", highlight) + " for " + MyText.Colofied(AllLvls[Index].Duration + " secs", highlight) + "\n\nCost: " + MyText.Colofied(AllLvls[Index].EssenseCost+" Essense",highlight) + "\nCD: " + MyText.Colofied(AllLvls[Index].CD + " secs",highlight);
    }

    public override void GenerateDescription() {
        Furylvl[] AllLvls = GetComponents<Furylvl>();
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
        FuryParticle = GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        Furylvl FL = null;
        switch (this.lvl) {
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
        EssenseCost = FL.EssenseCost;
        Duration = FL.Duration;
        AttkSpd_INC_Percentage = FL.AttkSpd_INC_Percentage;

        FuryParticle.startSize *= OC.GetVFXScale();

        GenerateDescription();
    }

    public override void Active() {
        OC.ON_ESSENSE_COST += OC.DeductEssense;
        OC.ON_ESSENSE_COST(new EssenseCost(EssenseCost,OC,typeof(Fury)));
        OC.ON_ESSENSE_COST -= OC.DeductEssense;
        ApplyFuryBuff();
        StartCoroutine(RunFuryParticleVFX(Duration));
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }

    private void ApplyFuryBuff() {
        // **Note** Could possibly need to check if OC has the buff already for network
        FuryBuff FB = FuryBuff.Generate(AttkSpd_INC_Percentage, Duration);
        FB.ApplyBuff(OC,OC);
        RealTime_CD =  CD - CD * (OC.GetMaxStats(STATSTYPE.HASTE) / 100);
    }

    IEnumerator RunFuryParticleVFX(float time) {
        FuryParticle.enableEmission = true;
        yield return new WaitForSeconds(time);
        FuryParticle.enableEmission = false;
    }
}
