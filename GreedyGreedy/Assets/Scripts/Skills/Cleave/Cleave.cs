using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;
public class Cleave : ActiveSkill {
    public GameObject HitVFX;
    [HideInInspector]
    public float DamageScale;
    [HideInInspector]
    public float ScalingFactor;

    private Animator Anim;
    private CleaveIndicator Indicator;

    public AudioClip SFX;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    string DescriptionTemplate(Cleavelvl[] AllLvls, int Index) {
        return "\nSummon a dark weapon to deal " + MyText.Colofied(AllLvls[Index].DamageScale + "%",highlight) + " + Dark Weapon Size * "+ MyText.Colofied(AllLvls[Index].DamageScale + "% damage", highlight) + " to enemies infront of you and knock them off. Dark weapon could be charged up to size 3, and the bigger weapon knock off enemies further.\n\nCost: " + MyText.Colofied(AllLvls[Index].EssenseCost + " Essense", highlight) + "\nCD: " + MyText.Colofied(AllLvls[Index].CD + " secs", highlight);
    }

    public override void GenerateDescription() {
        Cleavelvl[] AllLvls = GetComponents<Cleavelvl>();
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
        Anim = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Skill;
        Indicator = GetComponentInChildren<CleaveIndicator>(true);
    }

    protected override void Start() {
        base.Start();
    }


    protected override void Update() {
        base.Update();

    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        Cleavelvl CL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                CL = GetComponent<Cleave1>();
                break;
            case 2:
                CL = GetComponent<Cleave2>();
                break;
            case 3:
                CL = GetComponent<Cleave3>();
                break;
            case 4:
                CL = GetComponent<Cleave4>();
                break;
            case 5:
                CL = GetComponent<Cleave5>();
                break;
        }
        CD = CL.CD;
        EssenseCost = CL.EssenseCost;
        DamageScale = CL.DamageScale;
        transform.localScale = new Vector2(ScalingFactor, ScalingFactor);
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), OC.GetRootCollider());
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

        //Anim.SetInteger("Direction", OC.Direction);
        ScalingFactor = (float)System.Math.Round(Indicator.ScalingFactor, 1);
        transform.localScale = new Vector3(ScalingFactor, ScalingFactor, 0);
        transform.localEulerAngles = new Vector3(0, 0, Indicator.Z_Angle);
        Anim.SetTrigger("Active");
        StartCoroutine(Reset(1));

        OC.ON_ESSENSE_COST += OC.DeductEssense;
        OC.ON_ESSENSE_COST(new EssenseCost(EssenseCost,OC,typeof(Cleave)));
        OC.ON_ESSENSE_COST -= OC.DeductEssense;
        RealTime_CD = CD - CD * (OC.GetMaxStats(STATSTYPE.HASTE) / 100);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer(CollisionLayer.KillingGround))
            return;
        if (OC.GetType().IsSubclassOf(typeof(Player))) {//Player Attack
            if (collider.tag == Tag.FriendlyPlayer) {
                //if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))//Prevent duplicated attacks
                return;
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            Push(target);
            OC.ON_DMG_DEAL += DealCleaveDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealCleaveDmg;
            HittedStack.Push(collider);
        } else {
            if (collider.tag == Tag.Monster) {
                return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            Push(target);
            OC.ON_DMG_DEAL += DealCleaveDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealCleaveDmg;
            HittedStack.Push(collider);
        }
    }

    void DealCleaveDmg(ObjectController target) {
        float RawDamage;
        bool Crit;
        if (UnityEngine.Random.value < (OC.CurrCritChance / 100)) {
            RawDamage = OC.CurrDamage * (DamageScale / 100 + ScalingFactor*(DamageScale/100)) * (OC.CurrCritDmg / 100);
            Crit = true;
        } else {
            RawDamage = OC.CurrDamage * (DamageScale / 100 + ScalingFactor * (DamageScale / 100));
            Crit = false;
        }
        DirectDamage CleaveDmg = new DirectDamage(RawDamage, target.CurrDefense, OC.CurrPenetration, Crit, OC, typeof(Cleave));

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(new Value(OC.GetCurrLPH(), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ActiveOneShotVFXParticle(HitVFX);


        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(CleaveDmg);
        target.ON_DMG_TAKEN -= target.DeductHealth;
    }

    void Push(ObjectController target) {
        Vector2 BouceOffDirection = (Vector2)Vector3.Normalize(target.transform.position - OC.transform.position);
        target.NormalizeRigibody();
        target.AddForce(BouceOffDirection, Mathf.Floor(ScalingFactor) * 3+1, ForceMode2D.Impulse);
    }

    IEnumerator Reset(float time) {
        yield return new WaitForSeconds(time);
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
    }
}
