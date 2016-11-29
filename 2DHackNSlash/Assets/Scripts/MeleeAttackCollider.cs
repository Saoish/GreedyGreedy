using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeAttackCollider : AttackCollider {
    public float AttackRange = 0.1f;//Spawn offset: +x = right, -x = left, +y = up, -y = down
    public float AttackBoxWidth = 0.16f;
    public float AttackBoxHeight = 0.32f;

    BoxCollider2D SelfCollider;

    ObjectController OC;

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
        OC = transform.parent.GetComponent<ObjectController>();
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

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;

        if (OC.GetType().IsSubclassOf(typeof(PlayerController))) {
            if (dmg.IsCrit) {
                target.ActiveOneShotVFXParticle("WeaponCritSlashVFX", Layer.Skill);
            }
        } else {

        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("KillingGround"))
            return;
        if (OC.GetType().IsSubclassOf(typeof(PlayerController))) {//Player Attack
            if (collider.tag == "Player") {
                if (collider.GetComponent<ObjectController>().GetType() == typeof(FriendlyPlayer))
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
