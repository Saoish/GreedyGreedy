using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;
public class WarStomp : ActiveSkill {
    float ADScale;
    float StunDuration;
    float StompRadius;

    delegate void Del(ObjectController target);
    Del DEL;

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

    protected override void Awake() {
        base.Awake();
        StompCollider = GetComponent<CircleCollider2D>();
        StompVFX = transform.Find("War Stomp VFX").gameObject;
        StompVFX_DefaultStartSize = transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().startSize;
        PulseVFX_DefaultStartSize = transform.Find("War Stomp VFX/pulse").GetComponent<ParticleSystem>().startSize;
        SubEmitterBirthVFX_DefaultStartSize = transform.Find("War Stomp VFX/pulse/SubEmitterBirth").GetComponent<ParticleSystem>().startSize;
        VFX_StayTime = transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().duration;
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
        StompRadius = WSL.StompRadius;

        Physics2D.IgnoreCollision(StompCollider, OC.GetRootCollider());//Ignore self here

        StompCollider.radius = StompRadius;

        float ScaleFactor = StompCollider.radius / RadiusScaleFactor;
        transform.Find("War Stomp VFX").GetComponent<ParticleSystem>().startSize  = StompVFX_DefaultStartSize* ScaleFactor;
        transform.Find("War Stomp VFX/pulse").GetComponent<ParticleSystem>().startSize = PulseVFX_DefaultStartSize* ScaleFactor;
        transform.Find("War Stomp VFX/pulse/SubEmitterBirth").GetComponent<ParticleSystem>().startSize= SubEmitterBirthVFX_DefaultStartSize * ScaleFactor;

        Description = "Heavily stomp the ground, dealing "+ADScale+"% AD dmg to nearby foes and stun them for "+StunDuration+ " secs. Higher level gives you bigger stomp radius.\n\nCost: " + ManaCost+" Mana\nCD: "+CD+" secs";
    }

    public override void Active() {
        OC.ON_MANA_UPDATE += OC.DeductMana;
        OC.ON_MANA_UPDATE(new Value(ManaCost));
        OC.ON_MANA_UPDATE -= OC.DeductMana;
        StartCoroutine(ActiveStompCollider(StompTime));
        RealTime_CD = CD;
        StartCoroutine(RunStompVFX(VFX_StayTime));
        AudioSource.PlayClipAtPoint(StompSFX, transform.position, GameManager.SFX_Volume);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("KillingGround"))
            return;
        if (OC.GetType().IsSubclassOf(typeof(PlayerController))) {//Player Attack
            if (collider.tag == "Player") {
                if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } else if (HittedStack.Count != 0 && HittedStack.Contains(collider))
                return;
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();;
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
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += StunAndDealStompDmg;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= StunAndDealStompDmg;
            HittedStack.Push(collider);
        }
    }

    private void ApplyStunDebuff(ObjectController target) {
        StunDebuff SD = StunDebuff.Generate(StunDuration);
        SD.ApplyDebuff(target);
    }

    private void StunAndDealStompDmg(ObjectController target) {
        if (target.HasDebuff(typeof(StunDebuff))) {
            Debuff ExistedStunDebuff = target.GetDebuff(typeof(StunDebuff));
            if (StunDuration > ExistedStunDebuff.Duration)
                ExistedStunDebuff.Duration = StunDuration;
        } else {
            ApplyStunDebuff(target);
        }

        Value dmg = new Value(0, 0, false, OC);
        if(UnityEngine.Random.value < (OC.GetCurrStats(StatsType.CRIT_CHANCE) / 100)) {
            dmg.Amount += OC.GetCurrStats(StatsType.AD) * (ADScale / 100) * (OC.GetCurrStats(StatsType.CRIT_DMG) / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount += OC.GetCurrStats(StatsType.AD) * (ADScale / 100);
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (target.GetCurrStats(StatsType.DEFENSE) / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;

        //OC.ON_HEALTH_UPDATE += OC.HealHP;
        //OC.ON_HEALTH_UPDATE(new Value(OC.GetCurrStats(StatsType.HEALTH), 1));
        //OC.ON_HEALTH_UPDATE -= OC.HealHP;

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

    IEnumerator RunStompVFX(float time) {
        StompVFX.SetActive(true);
        yield return new WaitForSeconds(time);
        StompVFX.SetActive(false);
    }

}
