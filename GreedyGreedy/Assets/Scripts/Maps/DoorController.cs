using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {
    Transform GateCollider;
    [HideInInspector]
    MapController MC;
    // Use this for initialization
    void Start() {
        GateCollider = transform.Find("Gate Collider");
        MC = transform.parent.GetComponent<MapController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Close(){
        GateCollider.GetComponent<Collider2D>().enabled = true;
        ((AncientSlimeRoom)MC).StartWait();//For now
    }

    public void Open() {
        GateCollider.GetComponent<Collider2D>().enabled = false;
    }
}
