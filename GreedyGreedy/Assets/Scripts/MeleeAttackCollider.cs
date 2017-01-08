using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;

public class MeleeAttackCollider : AttackCollider {
    public float AttackRange = 0.1f;//Spawn offset: +x = right, -x = left, +y = up, -y = down
    public float AttackBoxWidth = 0.16f;
    public float AttackBoxHeight = 0.32f;

    public BoxTrigger BoxTrigger;
    

    public GameObject HitVFX;
    public AudioClip HitSFX;
    public List<AudioClip> DamageSFXList;

    BoxCollider2D SelfCollider;

    ObjectController OC;

    [HideInInspector]
    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer(CollisionLayer.Melee);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.Melee), LayerMask.NameToLayer(CollisionLayer.Loot));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.Melee), LayerMask.NameToLayer(CollisionLayer.Interaction));
        SelfCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Start() {
        base.Start();
        OC = GetComponentInParent<ObjectController>();
        if (OC != null) {
            Physics2D.IgnoreCollision(SelfCollider, OC.GetRootCollider());
            //if (OC.GetType() == typeof(MainPlayer))
            //    Owner = ObjectIdentity.Main;
            //else if (OC.GetType() == typeof(EnemyPlayer))
            //    Owner = ObjectIdentity.Enemy;
            //else if (OC.GetType() == typeof(FriendlyPlayer))
            //    Owner = ObjectIdentity.Friend;
            //else
            //    Owner = ObjectIdentity.Monster;                
        }
    }

    protected override void Update () {
        base.Update();
	}


    public void Active() {
        SelfCollider.enabled = true;
    }

    public void Deactive() {
        SelfCollider.enabled = false;
        if (HittedStack.Count != 0)
            HittedStack.Clear();
    }
    
    public BoxCollider2D MeleeCollder {
        get { return GetComponent<BoxCollider2D>(); }
    }

    DirectDamage AutoAttackDamageDeal(float TargetDefense) {
        if (Random.value < (OC.GetCurrStats(STATSTYPE.CRIT_CHANCE) / 100)) {
            float RawDamage = OC.GetCurrStats(STATSTYPE.DAMAGE) * (OC.GetCurrStats(STATSTYPE.CRIT_DMG) / 100);
            return new DirectDamage(RawDamage, TargetDefense, OC.GetCurrStats(STATSTYPE.PENETRATION), true, OC, typeof(MeleeAttackCollider));
        } else {            
            return new DirectDamage(OC.GetCurrStats(STATSTYPE.DAMAGE), TargetDefense, OC.GetCurrStats(STATSTYPE.PENETRATION), false, OC, typeof(MeleeAttackCollider));
        }
    }

    void DealMeleeAttackDMG(ObjectController target) {
        DirectDamage dmg = AutoAttackDamageDeal(target.GetCurrStats(STATSTYPE.DEFENSE));

        OC.ON_HEALTH_GAIN += OC.HealHP;
        OC.ON_HEALTH_GAIN(new HealHP(dmg.Amount*(OC.GetCurrStats(STATSTYPE.LPH)/100),dmg.Crit,OC, typeof(MeleeAttackCollider)));
        OC.ON_HEALTH_GAIN -= OC.HealHP;

        target.ON_DMG_TAKEN += target.DeductHealth;
        target.ON_DMG_TAKEN(dmg);
        target.ON_DMG_TAKEN -= target.DeductHealth;

        if (HitVFX != null)
            target.ActiveOneShotVFXParticle(HitVFX);
        if (DamageSFXList.Count>0) {
            AudioSource.PlayClipAtPoint(DamageSFXList[UnityEngine.Random.Range(0, DamageSFXList.Count)], target.transform.position, GameManager.SFX_Volume); 
        }
        else if (HitSFX != null)
            AudioSource.PlayClipAtPoint(HitSFX, target.transform.position, GameManager.SFX_Volume);
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer(CollisionLayer.KillingGround))
            return;
        if (OC.GetType().IsSubclassOf(typeof(Player))) {//Player Attack
            if (collider.tag == Tag.FriendlyPlayer) {
                //if (collider.transform.parent.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
                    return;
            } 
            else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                    return;
            }
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += DealMeleeAttackDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealMeleeAttackDMG;
            HittedStack.Push(collider);
        }
        else {//Enemy Attack
            if (collider.tag == Tag.Monster) {
                return;
            }
            else if(HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            ObjectController target = collider.transform.parent.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += DealMeleeAttackDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealMeleeAttackDMG;
            HittedStack.Push(collider);
        }
    }
}
