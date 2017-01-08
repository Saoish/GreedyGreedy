using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GreedyNameSpace;
public class BloodyHand : ActiveSkill {
    public float PullForce;

    float DamageScale;
    [HideInInspector]
    public float RangeScale = 1;

    BloodyHandIndicator Indicator;

    Animator Anim;
    public AudioClip SFX;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    string DescriptionTemplate(BloodyHandlvl[] AllLvls, int Index) {
        return "\nSummon a size " + MyText.Colofied(AllLvls[Index].RangeScale, highlight) + " bloody hand to deal " + MyText.Colofied(AllLvls[Index].DamageScale + "%", highlight) + " damage and grab enemies for you.\n\nCost: " + MyText.Colofied(AllLvls[Index].EssenseCost + " Essense", highlight) + "\nCD: " + MyText.Colofied(AllLvls[Index].CD + " secs", highlight);
    }

    public override void GenerateDescription() {
        BloodyHandlvl[] AllLvls = GetComponents<BloodyHandlvl>();
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
        Indicator = GetComponentInChildren<BloodyHandIndicator>(true);
    }

    protected override void Start() {
        base.Start();
    }


    protected override void Update() {
        base.Update();

    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC, lvl);
        BloodyHandlvl BHL = null;
        switch (this.lvl) {
            case 0:
                return;
            case 1:
                BHL = GetComponent<BloodyHand1>();
                break;
            case 2:
                BHL = GetComponent<BloodyHand2>();
                break;
            case 3:
                BHL = GetComponent<BloodyHand3>();
                break;
            case 4:
                BHL = GetComponent<BloodyHand4>();
                break;
            case 5:
                BHL = GetComponent<BloodyHand5>();
                break;
        }
        CD = BHL.CD;
        EssenseCost = BHL.EssenseCost;
        DamageScale = BHL.DamageScale;
        RangeScale = BHL.RangeScale;
        transform.localScale = new Vector2(RangeScale, RangeScale);
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
        transform.localEulerAngles = new Vector3(0, 0, Indicator.Z_Angle);
        Anim.SetTrigger("Active");
        StartCoroutine(Reset(1));

        OC.ON_ESSENSE_COST += OC.DeductEssense;
        OC.ON_ESSENSE_COST(new EssenseCost(EssenseCost,OC,typeof(BloodyHand)));
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
            Pull(target);
            OC.ON_DMG_DEAL += DealBHDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealBHDmg;
            HittedStack.Push(collider);
        } else {
            if (collider.tag == Tag.Monster) {
                return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
            Pull(target);
            OC.ON_DMG_DEAL += DealBHDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealBHDmg;
            HittedStack.Push(collider);
        }
    }

    void DealBHDmg(ObjectController target) {
        float RawDamage;
        bool Crit;
        if (UnityEngine.Random.value < (OC.CurrCritChance / 100)) {
            RawDamage = OC.CurrDamage * (DamageScale / 100) * (OC.CurrCritDmg / 100);
            Crit = true;
        } else {
            RawDamage = OC.CurrDamage * (DamageScale / 100);
            Crit = false;
        }
        DirectDamage BHDamage = new DirectDamage(RawDamage, target.CurrDefense, OC.CurrPenetration, Crit, OC, typeof(BloodyHand));

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(new Value(OC.GetCurrLPH(), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(BHDamage);
        target.ON_DMG_TAKEN -= target.DeductHealth;
    }

    void Pull(ObjectController target) {
        Vector2 PullDirection = (Vector2)Vector3.Normalize(OC.transform.position - target.transform.position);
        //target.NormalizeRigibody();
        target.AddForce(PullDirection,PullForce, ForceMode2D.Impulse);
    }

    IEnumerator Reset(float time) {
        yield return new WaitForSeconds(time);
        transform.localEulerAngles = Vector3.zero;
    }

}