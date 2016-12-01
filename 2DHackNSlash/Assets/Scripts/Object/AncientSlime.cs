using UnityEngine;
using System.Collections;
using System;

public class AncientSlime : EnemyController {
    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical("Giant Green Slime Explosion", Layer.Ground);
        Destroy(gameObject);
    }
}
