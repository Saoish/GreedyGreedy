using UnityEngine;
using System.Collections;
using System;

public class RedSlime : Monster {
    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical(DieVFX);
        Destroy(gameObject, 1f);
    }
}
