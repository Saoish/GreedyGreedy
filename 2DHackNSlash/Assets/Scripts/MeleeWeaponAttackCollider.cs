using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeaponAttackCollider : MeleeAttackCollider {
    PlayerController PC;
    WeaponController WC;


    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        if (transform.parent.parent != null) {
            PC = transform.parent.parent.GetComponent<PlayerController>();
            WC = transform.parent.GetComponent<WeaponController>();
        }
    }

    protected override void Update() {
    }


    void DealWeaponMeleeAttackDMG(ObjectController target) {
        Value dmg = PC.AutoAttackDamageDeal(target.GetCurrDefense());
        if (dmg.IsCrit) {
            target.ActiveOneShotVFXParticle("WeaponCritSlashVFX");
        }


        PC.ON_HEALTH_UPDATE += PC.HealHP;
        PC.ON_HEALTH_UPDATE(Value.CreateValue(PC.GetCurrLPH(), 1));
        PC.ON_HEALTH_UPDATE -= PC.HealHP;

        PC.ON_MANA_UPDATE += PC.HealMana;
        PC.ON_MANA_UPDATE(Value.CreateValue(PC.GetCurrMPH(), 1));
        PC.ON_MANA_UPDATE -= PC.HealMana;


        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Enemy") {
            if (HittedStack.Count!=0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            EnemyController Enemy = collider.GetComponent<EnemyController>();
            PC.ON_DMG_DEAL += DealWeaponMeleeAttackDMG;
            PC.ON_DMG_DEAL(Enemy);
            PC.ON_DMG_DEAL -= DealWeaponMeleeAttackDMG;
            HittedStack.Push(collider);
        }
        else if (collider.tag == "Player") {
        }
    }
}
