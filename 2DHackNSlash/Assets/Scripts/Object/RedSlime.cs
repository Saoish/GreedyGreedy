using UnityEngine;
using System.Collections;
using System;

public class RedSlime : EnemyController {
    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical("Red Slime Explosion", Layer.Ground);
        Destroy(gameObject);
    }
}
