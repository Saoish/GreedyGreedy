using UnityEngine;
using System.Collections;

public class LootUI : MonoBehaviour {
    LootNameController LNC;
	// Use this for initialization
	void Start () {
        LNC = GetComponentInChildren<LootNameController>(true);
        if (GameManager.Show_Names == 1)
            LNC.TurnOn();
        else
            LNC.TurnOff();
    }
	
	// Update is called once per frame
	void Update () {        
        LNC.ShowNameUpdate();       
    }
}
