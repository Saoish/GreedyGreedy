using UnityEngine;
using System.Collections;

public class LootUI : MonoBehaviour {
    GameObject Name;
	// Use this for initialization
	void Start () {
        GetComponent<Canvas>().sortingLayerName = Layer.Ground;
        Name = transform.Find("Name").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        NameUpdate();
    }

    void NameUpdate() {
        if (GameManager.Show_Names == 1) {
            Name.SetActive(true);
        } else {
            Name.SetActive(false);
        }
    }


}
