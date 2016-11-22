using UnityEngine;
using System.Collections;
using System;

public class AncientSlime : EnemyController {
    protected override void DieUpdate() {
        if (CurrHealth <= 0) {//Insert dead animation here
            Alive = false;
            SpawnEXP();
            GetComponent<DropList>().SpawnLoots();
            Destroy(transform.parent.gameObject);
        }
    }
}
