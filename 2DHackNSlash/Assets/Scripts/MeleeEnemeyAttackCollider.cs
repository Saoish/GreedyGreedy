using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemeyAttackCollider : MeleeAttackCollider {
    EnemyController EC;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        if (transform.parent!= null) {
            EC = transform.parent.GetComponent<EnemyController>();
        }
    }

    protected override void Update() {
    }

    void DealEnemyMeleeAttackDMG(ObjectController target) {
        Value dmg = EC.AutoAttackDamageDeal(target.GetCurrDefense());
        //if (dmg.IsCrit) {
        //    target.ActiveOneTimeVFX("WeaponCritSlashVFX");
        //}
        EC.ON_HEALTH_UPDATE += EC.HealHP;
        EC.ON_HEALTH_UPDATE(Value.CreateValue(EC.GetCurrLPH(),1));
        EC.ON_HEALTH_UPDATE -= EC.HealHP;

        EC.ON_MANA_UPDATE += EC.HealMana;
        EC.ON_MANA_UPDATE(Value.CreateValue(EC.GetCurrMPH(),1));
        EC.ON_MANA_UPDATE -= EC.HealMana;

        target.ON_HEALTH_UPDATE += target.DeductHealth;
        target.ON_HEALTH_UPDATE(dmg);
        target.ON_HEALTH_UPDATE -= target.DeductHealth;
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            if (HittedStack.Count != 0 && HittedStack.Contains(collider)) {//Prevent duplicated attacks
                return;
            }
            PlayerController Player = collider.GetComponent<PlayerController>();
            EC.ON_DMG_DEAL += DealEnemyMeleeAttackDMG;
            EC.ON_DMG_DEAL(Player);
            EC.ON_DMG_DEAL -= DealEnemyMeleeAttackDMG;
            HittedStack.Push(collider);
        } 
    }
}
