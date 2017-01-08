using UnityEngine;
using System.Collections;
using System;

public class GreenSlime : Monster {
    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical(DieVFX);
        Destroy(gameObject, 1f);
    }
}
