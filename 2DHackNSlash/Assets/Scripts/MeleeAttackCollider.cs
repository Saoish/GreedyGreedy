using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeAttackCollider : AttackCollider {
    public float AttackRange = 0.1f;//Spawn offset: +x = right, -x = left, +y = up, -y = down
    public float AttackBoxWidth = 0.16f;
    public float AttackBoxHeight = 0.32f;

    BoxCollider2D SelfCollider;

    ObjectController OC;
    WeaponController WC;

    [HideInInspector]
    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Melee");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Melee"), LayerMask.NameToLayer("Loot"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Melee"), LayerMask.NameToLayer("LootBox"));
        SelfCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Start() {
        base.Start();
        if (transform.parent.name != "EnemyController" && transform.parent.parent!=null) {//Player
            OC = transform.parent.parent.GetComponent<ObjectController>();
            WC = transform.parent.GetComponent<WeaponController>();
        } else
            OC = transform.parent.GetComponent<ObjectController>();//Enemy
        if(OC!=null)
            Physics2D.IgnoreCollision(SelfCollider, OC.transform.GetComponent<Collider2D>());
    }

    protected override void Update () {
        base.Update();
	}

    void DealMeleeAttackDMG(ObjectController target) {
        Value dmg = OC.AutoAttackDamageDeal(target.GetCurrDefense());

        OC.ON_HEALTH_UPDATE += OC.HealHP;
        OC.ON_HEALTH_UPDATE(Value.CreateValue(OC.GetCurrLPH(), 1));
        OC.ON_HEALTH_UPDATE -= OC.HealHP;

        OC.ON_MANA_UPDATE += OC.HealMana;
        OC.ON_MANA_UPDATE(Value.CreateValue(OC.GetCurrMPH(), 1));
        OC.ON_MANA_UPDATE -= OC.HealMana;


        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;

        if (OC.GetType() == typeof(PlayerController)) {
            if (dmg.IsCrit) {
                target.ActiveOneShotVFXParticle("WeaponCritSlashVFX");
            }
        } else {

        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("KillingGround"))
            return;
        if (OC.GetType() == typeof(PlayerController)) {//Player Attack
            if (collider.tag == "Player") {
                if(collider.transform.parent.name == "FriendlyPlayer")
                    return;
            } 
            else if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                    return;
            }
            ObjectController target = collider.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += DealMeleeAttackDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealMeleeAttackDMG;
            HittedStack.Push(collider);
        }
        else {//Enemy Attack
            if (collider.tag == "Enemy") {
                return;
            }
            else if(HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            ObjectController target = collider.GetComponent<ObjectController>();
            OC.ON_DMG_DEAL += DealMeleeAttackDMG;
            OC.ON_DMG_DEAL(target);
            OC.ON_DMG_DEAL -= DealMeleeAttackDMG;
            HittedStack.Push(collider);
        }
    }
}
