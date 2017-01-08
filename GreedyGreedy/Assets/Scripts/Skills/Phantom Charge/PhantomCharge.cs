using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;
public class PhantomCharge : ActiveSkill {
    public GameObject EndSmoke;
    public AudioClip SFX;
    public AudioClip HitSFX;
    public float SmokeStayTime;
    public float EndStayTime;

    public float StunDuration = 1f;

    [HideInInspector]
    public float DamageScale;
    [HideInInspector]
    public float Force;

    ParticleSystem SmokePS;

    Collider2D ChargeCollider;

    PhantomChargeIndicator Indicator;
    //public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    string DescriptionTemplate(PhantomChargelvl[] AllLvls, int Index) {
        return "\nTurn into a black mist and charge forward with speed of " + MyText.Colofied(AllLvls[Index].Force, highlight) + ", dealing " + MyText.Colofied(AllLvls[Index].DamageScale + "%", highlight) + " damage to collided enemies and stun them for " + StunDuration + " sec(s).\n\n Cost: " + MyText.Colofied(AllLvls[Index].EssenseCost + " Essense", highlight) + "\nCD: " + MyText.Colofied(AllLvls[Index].CD + "secs", highlight);
    }

    public override void GenerateDescription() {
        PhantomChargelvl[] AllLvls = GetComponents<PhantomChargelvl>();
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
        SmokePS = transform.GetComponent<ParticleSystem>();
        SmokePS.GetComponent<Renderer>().sortingLayerName = SortingLayer.Skill;
        ChargeCollider = GetComponent<Collider2D>();
        Indicator = GetComponentInChildren<PhantomChargeIndicator>(true);
    }
    // Use this for initialization
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        PhantomChargelvl PCL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                PCL = GetComponent<PhantomCharge1>();
                break;
            case 2:
                PCL = GetComponent<PhantomCharge2>();
                break;
            case 3:
                PCL = GetComponent<PhantomCharge3>();
                break;
            case 4:
                PCL = GetComponent<PhantomCharge4>();
                break;
            case 5:
                PCL = GetComponent<PhantomCharge5>();
                break;
        }
        CD = PCL.CD;
        EssenseCost = PCL.EssenseCost;
        DamageScale = PCL.DamageScale;
        Force = PCL.Force;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), OC.GetRootCollider());//Ignore self here
        SmokePS.startSize *= OC.GetVFXScale();

        GenerateDescription();
    }

    public override void ActiveIndicator() {
        Indicator.Active(OC.Direction);
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

        SmokePS.enableEmission = true;
        //ChargeCollider.enabled = true;
        StartCoroutine(ActiveChargeCollider(0.1f));
        
        OC.ZerolizeDrag();
        OC.AddForce(Indicator.CastVector ,Force, ForceMode2D.Impulse);
        //Debug.Log(OC.rb.velocity);
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);

        OC.ON_ESSENSE_COST += OC.DeductEssense;
        OC.ON_ESSENSE_COST(new EssenseCost(EssenseCost,OC,typeof(PhantomCharge)));
        OC.ON_ESSENSE_COST -= OC.DeductEssense;

        RealTime_CD = CD - CD * (OC.GetMaxStats(STATSTYPE.HASTE) / 100);
    }

    void StunAndDealChargeDmg(ObjectController target) {
        if (target.HasDebuff(typeof(StunDebuff))) {
            Debuff ExistedStunDebuff = target.GetDebuff(typeof(StunDebuff));
            if (StunDuration > ExistedStunDebuff.Duration)
                ExistedStunDebuff.Duration = StunDuration;
        } else {
            ApplyStunDebuff(target);
        }

        float RawDamage;
        bool Crit;
        if (UnityEngine.Random.value < (OC.CurrCritChance / 100)) {
            RawDamage = OC.CurrDamage * (DamageScale / 100) * (OC.CurrCritDmg / 100);
            Crit = true;
        } else {
            RawDamage = OC.CurrDamage * (DamageScale / 100);
            Crit = false;
        }
        DirectDamage ChargedDmg = new DirectDamage(RawDamage, target.CurrDefense, OC.CurrPenetration, Crit, OC, typeof(PhantomCharge));

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(new Value(OC.GetCurrLPH(), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(ChargedDmg);
        target.ON_DMG_TAKEN -= target.DeductHealth;
    }

    void OnTriggerStay2D(Collider2D collider) {
        BoxCollider2D SelfMelee = OC.Melee_AC.MeleeCollder;
        if (collider.gameObject.layer == LayerMask.NameToLayer(CollisionLayer.Melee)) {//Ignore melee collider)
            if (collider != SelfMelee) {
                Debug.Log(collider.gameObject);
                return;
            }
        }
        if (OC.GetType().IsSubclassOf(typeof(Player))) {
            if (collider.tag == Tag.Monster || collider.tag == Tag.EnemyPlayer) {
                //if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                //    return;
                //}
                ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
                //target.MountainlizeMass();
                OC.ON_DMG_DEAL += StunAndDealChargeDmg;
                OC.ON_DMG_DEAL(target);
                OC.ON_DMG_DEAL -= StunAndDealChargeDmg;
                //HittedStack.Push(collider);                
                AudioSource.PlayClipAtPoint(HitSFX, transform.position, GameManager.SFX_Volume);
            } 
        }
        else {
            if (collider.tag !=Tag.Monster) {
                //if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                //    return;
                //}
                ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
                //target.MountainlizeMass();
                OC.ON_DMG_DEAL += StunAndDealChargeDmg;
                OC.ON_DMG_DEAL(target);
                OC.ON_DMG_DEAL -= StunAndDealChargeDmg;
                //HittedStack.Push(collider);
                AudioSource.PlayClipAtPoint(HitSFX, transform.position, GameManager.SFX_Volume);
            } 
        }

        OC.NormalizeDrag();
        OC.ZerolizeForce();
        ChargeCollider.enabled = false;
        //HittedStack.Clear();
        OC.ActiveVFXParticalWithStayTime(EndSmoke, EndStayTime);
        StartCoroutine(DisableSmokePS(SmokeStayTime));
    }

    IEnumerator DisableSmokePS(float time) {        
        yield return new WaitForSeconds(time);
        SmokePS.enableEmission = false;
    }

    IEnumerator ActiveChargeCollider(float time) {
        yield return new WaitForSeconds(time);
        ChargeCollider.enabled = true;
    }


    private void ApplyStunDebuff(ObjectController target) {
        StunDebuff SD = StunDebuff.Generate(StunDuration);
        SD.ApplyDebuff(OC,target);
    }
}