using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BloodyHand : ActiveSkill {
    public float PullForce;

    float ADScale;
    [HideInInspector]
    public float RangeScale = 1;

    Animator Anim;
    public AudioClip SFX;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        Anim = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingOrder = Layer.Skill;
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
        switch (this.SD.lvl) {
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
        ManaCost = BHL.ManaCost;
        ADScale = BHL.ADScale;
        RangeScale = BHL.RangeScale;
        transform.localScale = new Vector2(RangeScale, RangeScale);
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), OC.GetRootCollider());

        Description = "Summon a size " +RangeScale+" bloody hand to deal " +ADScale+"% AD damage and grab enemies for you.\n\nCost: "+ManaCost+" Mana\nCD: "+CD+" secs";
    }

    public override void Active() {
        Anim.SetInteger("Direction", OC.Direction);
        Anim.SetTrigger("Active");
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;
        RealTime_CD = CD;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("KillingGround"))
            return;
        if (OC.GetType().IsSubclassOf(typeof(PlayerController))) {//Player Attack
            if (collider.tag == "Player") {
                if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
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
            if (collider.tag == "Enemy") {
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
        Value dmg = Value.CreateValue(0, 0, false, OC);
        if (UnityEngine.Random.value < (OC.GetCurrCritChance() / 100)) {
            dmg.Amount += OC.GetCurrAD() * (ADScale / 100) * (OC.GetCurrCritDmgBounus() / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount += OC.GetCurrAD() * (ADScale / 100);
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (target.GetCurrDefense() / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;

        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(Value.CreateValue(OC.GetCurrLPH(), 1));
        OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    void Pull(ObjectController target) {
        Vector2 PullDirection = (Vector2)Vector3.Normalize(OC.transform.position - target.transform.position);
        //target.NormalizeRigibody();
        target.AddForce(PullDirection,PullForce, ForceMode2D.Impulse);
    }

}