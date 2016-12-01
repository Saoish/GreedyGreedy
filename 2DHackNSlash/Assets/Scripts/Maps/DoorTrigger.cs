using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

    DoorController DC;

	// Use this for initialization
	void Start () {
        DC = transform.parent.GetComponent<DoorController>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            DC.Close();
            gameObject.SetActive(false);
        }
    }
}
