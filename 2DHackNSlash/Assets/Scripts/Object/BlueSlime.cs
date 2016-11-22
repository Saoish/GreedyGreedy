using UnityEngine;
using System.Collections;

public class BlueSlime : EnemyController {
    protected override void DieUpdate() {
        if (CurrHealth <= 0) {//Insert dead animation here
            Alive = false;
            SpawnEXP();
            GetComponent<DropList>().SpawnLoots();
            Destroy(transform.parent.gameObject);
        }
    }

}
