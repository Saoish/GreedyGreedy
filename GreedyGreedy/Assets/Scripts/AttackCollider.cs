using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public abstract class AttackCollider : MonoBehaviour {
    //public ObjectIdentity Owner;
    protected virtual void Awake() {

    }

    protected virtual void Start() {

    }

    protected virtual void Update() {

    }

    protected abstract void OnTriggerEnter2D(Collider2D collider);
}
