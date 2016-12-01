using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cleave : ActiveSkill {
    [HideInInspector]
    public float ADScale;
    [HideInInspector]
    public float RangeScale;

    private Animator Anim;

    public AudioClip SFX;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        Anim = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingLayerName = Layer.Skill;
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
        switch (this.SD.lvl) {
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
        ManaCost = CL.ManaCost;
        ADScale = CL.ADScale;
        RangeScale = CL.RangeScale;
        transform.localScale = new Vector2(RangeScale, RangeScale);
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), OC.GetRootCollider());

        Description = "Summon a size "+RangeScale+" dark weapon to deal "+ ADScale+ "% AD damage to enemies infront of you and knock them off. \n\nCost: " + ManaCost + " Mana\nCD: "+CD+" secs\n";
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
            Push(target);
            OC.ON_DMG_DEAL += DealCleaveDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealCleaveDmg;
            HittedStack.Push(collider);
        } else {
            if (collider.tag == "Enemy") {
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

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(Value.CreateValue(OC.GetCurrLPH(), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

        if (dmg.IsCrit) {
            target.ActiveOneShotVFXParticle("WeaponCritSlashVFX");
        }

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    void Push(ObjectController target) {
        Vector2 BouceOffDirection = (Vector2)Vector3.Normalize(target.transform.position - OC.transform.position);
        target.NormalizeRigibody();
        target.AddForce(BouceOffDirection,SD.lvl * 2, ForceMode2D.Impulse);
    }

    void UpdateSkillInfo() {

    }
}
