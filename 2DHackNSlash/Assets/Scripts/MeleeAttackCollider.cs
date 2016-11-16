using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeAttackCollider : AttackCollider {
    public float AttackRange = 0.1f;//Spawn offset: +x = right, -x = left, +y = up, -y = down
    public float AttackBoxWidth = 0.16f;
    public float AttackBoxHeight = 0.32f;

    BoxCollider2D AttackCollider;

    [HideInInspector]
    public Stack<Collider2D> HittedStack = new Stack<Collider2D>();

    protected override void Awake() {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Melee");
        AttackCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update () {
        base.Update();
	}

    protected override void OnTriggerEnter2D(Collider2D collider) {
        base.OnTriggerEnter2D(collider);
    }
}
