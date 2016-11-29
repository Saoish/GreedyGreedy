using UnityEngine;
using System.Collections;

public class ContactDetector : MonoBehaviour {
    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("ContactDetector");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ContactDetector"), LayerMask.NameToLayer("Loot"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ContactDetector"), LayerMask.NameToLayer("Skill"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ContactDetector"), LayerMask.NameToLayer("Melee"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ContactDetector"), LayerMask.NameToLayer("Projectile"));
        Physics2D.IgnoreCollision(transform.Find("PlayerController").GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Enemy" || collider.tag == "Player") {
            collider.transform.GetComponent<ObjectController>().MountainlizeMass();
            collider.transform.GetComponent<ObjectController>().ZerolizeForce();
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.tag == "Enemy" || collider.tag == "Player") {
            collider.transform.GetComponent<ObjectController>().NormalizeMass();
        }
    }
}
