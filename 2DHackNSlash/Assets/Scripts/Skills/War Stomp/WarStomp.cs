using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarStomp : ActiveSkill {
    float ADScale;
    float StunDuration;

    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    Collider2D StompCollider;

    public AudioClip StompSFX;
    public float StompTime = 0.1f;

    protected override void Awake() {
        base.Awake();
        StompCollider = GetComponent<Collider2D>();
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
        switch (this.SD.lvl) {
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
        ManaCost = WSL.ManaCost;
        ADScale = WSL.ADScale;
        StunDuration = WSL.StunDuration;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), OC.transform.GetComponent<Collider2D>());//Ignore self here

        Description = "Heavily stomp the ground, dealing "+ADScale+"% AD dmg to nearby foes and stun them for "+StunDuration+" secs.\n\nCost: "+ManaCost+" Mana\nCD: "+CD+" secs";
    }

    public override void Active() {
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;
        StartCoroutine(ActiveStompCollider(StompTime));
        RealTime_CD = CD;
        AudioSource.PlayClipAtPoint(StompSFX, transform.position, GameManager.SFX_Volume);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("KillingGround"))
            return;
        if (OC.GetType().IsSubclassOf(typeof(PlayerController))) {//Player Attack
            if (collider.tag == "Player") {
                if (collider.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))
                return;
            ObjectController target = collider.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += StunAndDealStompDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= StunAndDealStompDmg;
            HittedStack.Push(collider);
        } else {
            if(collider.tag == "Enemy") {
                return;
            }
            else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {
                return;
            }
            ObjectController target = collider.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += StunAndDealStompDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= StunAndDealStompDmg;
            HittedStack.Push(collider);
        }
    }

    private void ApplyStunDebuff(ObjectController target) {
        ModData StunDebuffMod = ScriptableObject.CreateInstance<ModData>();
        StunDebuffMod.Name = "StunDebuff";
        StunDebuffMod.Duration = StunDuration;
        GameObject StunDebuffObject = Instantiate(Resources.Load("DebuffPrefabs/"+ StunDebuffMod.Name)) as GameObject;
        StunDebuffObject.name = StunDebuffMod.Name;
        StunDebuffObject.GetComponent<Debuff>().ApplyDebuff(StunDebuffMod, target);

    }

    private void StunAndDealStompDmg(ObjectController target) {
        if (target.HasDebuff(typeof(StunDebuff))) {
            Debuff ExistedStunDebuff = target.GetDebuff(typeof(StunDebuff));
            if (StunDuration > ExistedStunDebuff.Duration)
                ExistedStunDebuff.Duration = StunDuration;
        } else {
            ApplyStunDebuff(target);
        }

        Value dmg = Value.CreateValue(0, 0, false, OC);
        if(UnityEngine.Random.value < (OC.GetCurrCritChance() / 100)) {
            dmg.Amount += OC.GetCurrAD() * (ADScale / 100) * (OC.GetCurrCritDmgBounus() / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount += OC.GetCurrAD() * (ADScale / 100);
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (target.GetCurrDefense() / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;

        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(Value.CreateValue(OC.GetCurrLPH(),1));
        OC.ON_HEALTH_UPDATE -= OC.HealHP;

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    IEnumerator ActiveStompCollider(float time) {
        StompCollider.enabled = true;
        yield return new WaitForSeconds(time);
        StompCollider.enabled = false;
        HittedStack.Clear();
    }

}
