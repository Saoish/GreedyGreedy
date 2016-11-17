using UnityEngine;
using System.Collections;

public abstract class AttackCollider : MonoBehaviour {

    protected virtual void Awake() {

    }

    protected virtual void Start() {

    }

    protected virtual void Update() {

    }

    protected abstract void OnTriggerEnter2D(Collider2D collider);
}
