using UnityEngine;
using System.Collections;
using System;

public class BlueSlime : EnemyController {
    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical("Blue Slime Explosion", Layer.Ground);
        Destroy(gameObject);
    }
}
