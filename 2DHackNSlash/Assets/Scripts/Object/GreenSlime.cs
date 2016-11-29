using UnityEngine;
using System.Collections;
using System;

public class GreenSlime : EnemyController {
    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical("Green Slime Explosion", Layer.Ground);
    }
}
