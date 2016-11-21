using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhantomCharge : ActiveSkill {
    public AudioClip SFX;
    public AudioClip Hit;
    public float SmokeStayTime;
    public float EndStayTime;

    [HideInInspector]
    public float ADScale;
    [HideInInspector]
    public float Force;

    ParticleSystem SmokePS;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        SmokePS = transform.GetComponent<ParticleSystem>();
    }
    // Use this for initialization
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void InitSkill(int lvl) {
        base.InitSkill(lvl);
        PhantomChargelvl PCL = null;
        switch (this.SD.lvl) {
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
        ManaCost = PCL.ManaCost;
        ADScale = PCL.ADScale;
        Force = PCL.Force;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), OC.transform.GetComponent<Collider2D>());//Ignore self here
    }

    public override void Active() {
        Vector2 charge_direction = Vector2.zero;
        if (OC.MoveVector == Vector2.zero) {
            switch (OC.Direction) {
                case 0:
                    charge_direction = new Vector2(0, -1);
                    break;
                case 1:
                    charge_direction = new Vector2(-1, 0);
                    break;
                case 2:
                    charge_direction = new Vector2(1, 0);
                    break;
                case 3:
                    charge_direction = new Vector2(0, 1);
                    break;
            }
        } else {
            charge_direction = OC.MoveVector;
        }

        SmokePS.enableEmission = true;
        transform.GetComponent<Collider2D>().enabled = true;
        OC.ZerolizeDrag();
        OC.AddForce(charge_direction ,Force, ForceMode2D.Impulse);
        //Debug.Log(OC.rb.velocity);
        AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);

        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;

        RealTime_CD = CD;
    }

    void DealSkillDmg(ObjectController target) {
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

        OC.ON_MANA_UPDATE += OC.HealMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(OC.GetCurrMPH(), 1));
        OC.ON_MANA_UPDATE -= OC.HealMana;

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Melee")) {//Ignore melee collider)
            if (!collider.transform.IsChildOf(OC.transform))
                return;
        }
        if (OC.GetType() == typeof(PlayerController)) {
            if (collider.tag == "Enemy") {
                if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                    return;
                }
                ObjectController target = collider.GetComponent<ObjectController>();
                //target.MountainlizeMass();
                OC.ON_DMG_DEAL += DealSkillDmg;
                OC.ON_DMG_DEAL(target);
                OC.ON_DMG_DEAL -= DealSkillDmg;
                HittedStack.Push(collider);
                AudioSource.PlayClipAtPoint(Hit, transform.position, GameManager.SFX_Volume);
            } else if (collider.tag == "Player") {
                if (collider.transform.parent.name != "FriendlyPlayer") {
                    if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                        return;
                    }
                    ObjectController target = collider.GetComponent<ObjectController>();
                    target.MountainlizeMass();
                    OC.ON_DMG_DEAL += DealSkillDmg;
                    OC.ON_DMG_DEAL(target);
                    OC.ON_DMG_DEAL -= DealSkillDmg;
                    HittedStack.Push(collider);
                    AudioSource.PlayClipAtPoint(Hit, transform.position, GameManager.SFX_Volume);
                }
            }
        }


        else {
            if (collider.tag == "Player") {
                if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                    return;
                }
                ObjectController target = collider.GetComponent<ObjectController>();
                //target.MountainlizeMass();
                OC.ON_DMG_DEAL += DealSkillDmg;
                OC.ON_DMG_DEAL(target);
                OC.ON_DMG_DEAL -= DealSkillDmg;
                HittedStack.Push(collider);
                AudioSource.PlayClipAtPoint(Hit, transform.position, GameManager.SFX_Volume);
            } 
        }

        OC.NormalizeDrag();
        OC.ZerolizeForce();
        transform.GetComponent<Collider2D>().enabled = false;
        HittedStack.Clear();
        OC.ActiveVFXParticalWithStayTime("Phantom Charge End", EndStayTime);
        StartCoroutine(DisableSmokePS(SmokeStayTime));
    }

    IEnumerator DisableSmokePS(float time) {
        yield return new WaitForSeconds(time);
        SmokePS.enableEmission = false;
    }

}