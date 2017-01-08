using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContactDetector : MonoBehaviour {
    public MainPlayer MP;
    List<ObjectController> Contacts;

    //[HideInInspector]
    public bool ContactDown = false;
    //[HideInInspector]
    public bool ContactLeft = false;
    //[HideInInspector]
    public bool ContactRight = false;
    //[HideInInspector]
    public bool ContactTop = false;

    void Awake() {
        Contacts = new List<ObjectController>();
        gameObject.layer = LayerMask.NameToLayer(CollisionLayer.ContactDetector);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.ContactDetector), LayerMask.NameToLayer(CollisionLayer.Loot));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.ContactDetector), LayerMask.NameToLayer(CollisionLayer.Skill));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.ContactDetector), LayerMask.NameToLayer(CollisionLayer.Melee));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.ContactDetector), LayerMask.NameToLayer(CollisionLayer.Projectile));
        Physics2D.IgnoreCollision(transform.Find("Root").GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    void Update() {
        ProcessContacts();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == Tag.Monster || collider.tag == Tag.FriendlyPlayer || collider.tag == Tag.EnemyPlayer) {            
            collider.transform.parent.GetComponent<ObjectController>().ZerolizeForce();
            collider.transform.parent.GetComponent<ObjectController>().MountainlizeRigibody();
            ObjectController OC = collider.transform.parent.GetComponent<ObjectController>();
            if (!Contacts.Contains(OC)) {
                Contacts.Add(OC);                
            }
        }        
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.tag == Tag.Monster || collider.tag == Tag.FriendlyPlayer || collider.tag == Tag.EnemyPlayer) {
            collider.transform.parent.GetComponent<ObjectController>().NormalizeRigibody();            
            Contacts.Remove(collider.transform.parent.GetComponent<ObjectController>());
        }
    }

    void ProcessContacts() {
        if (Contacts.Count == 0) {
            ContactTop = false;
            ContactDown = false;
            ContactLeft = false;
            ContactRight = false;  
            return;
        }
        List<ObjectController> Expired = new List<ObjectController>();
        foreach (var oj in Contacts) {
            if (!oj || !oj.Alive) {
                Expired.Add(oj);                
            } else {
                Vector2 ContactDirection = Vector3.Normalize(oj.Position - MP.Position);
                if (ContactDirection.x == 0) {
                    ContactLeft = ContactRight = false;                    
                }if (ContactDirection.y == 0){
                    ContactTop = ContactDown = false;
                }if (ContactDirection.x < -0.7f) {
                    ContactLeft = true;
                }if (ContactDirection.x > 0.7f) {
                    ContactRight = true;
                }if (ContactDirection.y < -0.7f) {
                    ContactDown = true;
                }if (ContactDirection.y > 0.7f) {
                    ContactTop = true;
                }
            }
        }
        if(Expired.Count>0) {
            foreach (var exipire_oj in Expired)
                Contacts.Remove(exipire_oj);
        }
    }
    public bool HasContacted {
        get { return Contacts.Count > 0; }
    }
}
