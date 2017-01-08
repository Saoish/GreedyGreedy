using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;
public class WarStomp : ActiveSkill {
    float DamageScale;
    float StunDuration;
    float ScalingFactor;
    //float StompRadius;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    CircleCollider2D StompCollider;

    public AudioClip StompSFX;
    public float StompTime = 0.1f;

    GameObject StompVFX;

    float VFX_StayTime;
    float StompVFX_DefaultStartSize;
    float PulseVFX_DefaultStartSize;
    float SubEmitterBirthVFX_DefaultStartSize;

    float RadiusScaleFactor = 0.4f;

    WarStompIndicator Indicator;

    string DescriptionTemplate(WarStomplvl[] AllLvls, int Index) {
        return "\nHeavily stomp the ground, dealing " + MyText.Colofied(AllLvls[Index].DamageScale+ "%",highlight) + " * Stomp Charge (Up to 2) damage to nearby foes and stun them for " + MyText.Colofied(AllLvls[Index].StunDuration + " secs", highlight) + ". Chagre it for bigger stomp.\n\nCost: " + MyText.Colofied(AllLvls[Index].EssenseCost + " Essense", highlight) + "\nCD: " + MyText.Colofied(AllLvls[Index].CD + " secs", highlight);
    }

    public override void GenerateDescription() {
        WarStomplvl[] AllLvls = GetComponents<WarStomplvl>();
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
        StompCollider = GetComponent<CircleCollider2D>();
        StompVFX = transform.Find("War Stomp VFX").gameObject;
        StompVFX_DefaultStartSize = transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().startSize;
        PulseVFX_DefaultStartSize = transform.Find("War Stomp VFX/pulse").GetComponent<ParticleSystem>().startSize;
        SubEmitterBirthVFX_DefaultStartSize = transform.Find("War Stomp VFX/pulse/SubEmitterBirth").GetComponent<ParticleSystem>().startSize;
        VFX_StayTime = transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().duration;
        Indicator = GetComponentInChildren<WarStompIndicator>(true);
    }

    protected override void Start() {
        base.Start();
    }


    protected override void Update() {
        base.Update();

    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        WarStomplvl WSL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                WSL = GetComponent<WarStomp1>();
                break;
            case 2:
                WSL = GetComponent<WarStomp2>();
                break;
            case 3:
                WSL = GetComponent<WarStomp3>();
                break;
            case 4:
                WSL = GetComponent<WarStomp4>();
                break;
            case 5:
                WSL = GetComponent<WarStomp5>();
                break;
        }

        CD = WSL.CD;
        EssenseCost = WSL.EssenseCost;
        DamageScale = WSL.DamageScale;
        StunDuration = WSL.StunDuration;
        //StompRadius = WSL.StompRadius;

        Physics2D.IgnoreCollision(StompCollider, OC.GetRootCollider());//Ignore self here

        //StompCollider.radius = StompRadius;

        //float ScaleFactor = StompCollider.radius / RadiusScaleFactor;
        //transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().startSize  = StompVFX_DefaultStartSize* ScaleFactor;
        //transform.Find("War Stomp VFX/pulse").GetComponent<ParticleSystem>().startSize = PulseVFX_DefaultStartSize* ScaleFactor;
        //transform.Find("War Stomp VFX/pulse/SubEmitterBirth").GetComponent<ParticleSystem>().startSize= SubEmitterBirthVFX_DefaultStartSize * ScaleFactor;

        GenerateDescription();
    }

    public override void ActiveIndicator() {
        Indicator.Active();
        Indicating = true;
        OC.Casting = true;
    }

    public override void Interrupt() {
        Indicator.Deactive();
        Indicating = false;
        OC.Casting = false;
    }

    public override void Active() {
        Indicator.Deactive();
        Indicating = false;
        OC.Casting = false;

        OC.ON_ESSENSE_COST += OC.DeductEssense;
        OC.ON_ESSENSE_COST(new EssenseCost(EssenseCost,OC,typeof(WarStomp)));
        OC.ON_ESSENSE_COST -= OC.DeductEssense;

        ScalingFactor = (float)System.Math.Round(Indicator.ScalingFactor, 1);
        StompCollider.radius = ScalingFactor * RadiusScaleFactor;
        transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().startSize  = StompVFX_DefaultStartSize* ScalingFactor;
        transform.Find("War Stomp VFX/pulse").GetComponent<ParticleSystem>().startSize = PulseVFX_DefaultStartSize* ScalingFactor;
        transform.Find("War Stomp VFX/pulse/SubEmitterBirth").GetComponent<ParticleSystem>().startSize= SubEmitterBirthVFX_DefaultStartSize * ScalingFactor;

        StartCoroutine(ActiveStompCollider(StompTime));
        RealTime_CD =  CD - CD * (OC.GetMaxStats(STATSTYPE.HASTE) / 100);
        StartCoroutine(RunStompVFX(VFX_StayTime));
        AudioSource.PlayClipAtPoint(StompSFX, transform.position, GameManager.SFX_Volume);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer(CollisionLayer.KillingGround))
            return;
        if (OC.GetType().IsSubclassOf(typeof(Player))) {//Player Attack
            if (collider.tag == Tag.FriendlyPlayer) {
                //if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))
                return;
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            OC.ON_DMG_DEAL += StunAndDealStompDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= StunAndDealStompDmg;
            HittedStack.Push(collider);
        } else {
            if(collider.tag == Tag.Monster) {
                return;
            }
            else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {
                return;
            }
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += StunAndDealStompDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= StunAndDealStompDmg;
            HittedStack.Push(collider);
        }
    }

    private void ApplyStunDebuff(ObjectController target) {
        StunDebuff SD = StunDebuff.Generate(StunDuration);
        SD.ApplyDebuff(OC,target);
    }

    private void StunAndDealStompDmg(ObjectController target) {
        if (target.HasDebuff(typeof(StunDebuff))) {
            Debuff ExistedStunDebuff = target.GetDebuff(typeof(StunDebuff));
            if (StunDuration > ExistedStunDebuff.Duration)
                ExistedStunDebuff.Duration = StunDuration;
        } else {
            ApplyStunDebuff(target);
        }

        float RawDamage;
        bool Crit;
        if(UnityEngine.Random.value < (OC.GetCurrStats(STATSTYPE.CRIT_CHANCE) / 100)) {
            RawDamage = OC.CurrDamage * (DamageScale * ScalingFactor / 100) * (OC.CurrCritDmg / 100);
            Crit = true;
        } else {
            RawDamage = OC.CurrDamage * (DamageScale * ScalingFactor / 100);
            Crit = false;
        }
        DirectDamage StompDmg = new DirectDamage(RawDamage, target.CurrDefense, OC.CurrPenetration, Crit, OC, typeof(WarStomp));

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(new Value(OC.GetCurrStats(StatsType.HEALTH), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(StompDmg);
        target.ON_DMG_TAKEN -= target.DeductHealth;
    }

    IEnumerator ActiveStompCollider(float time) {
        StompCollider.enabled = true;
        yield return new WaitForSeconds(time);
        StompCollider.enabled = false;
        HittedStack.Clear();
    }

    IEnumerator RunStompVFX(float time) {
        StompVFX.SetActive(true);
        yield return new WaitForSeconds(time);
        StompVFX.SetActive(false);
    }

}
