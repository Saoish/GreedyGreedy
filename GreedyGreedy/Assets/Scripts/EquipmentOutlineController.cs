using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class EquipmentOutlineController : MonoBehaviour {
    Equipment E;
    SpriteRenderer SR;
	// Use this for initialization
    void OnEnable() {
        if (E == null) {
            E = GetComponentInParent<EquipmentController>().E;
        }
    }
	void Start () {
        //E = GetComponentInParent<EquipmentController>().E;
        SR = GetComponent<SpriteRenderer>();
        switch (E.Rarity) {
            case RARITY.Common:
                SR.color = MyColor.Common;
                break;
            case RARITY.Fine:
                SR.color = MyColor.Fine;
                break;
            case RARITY.Pristine:
                SR.color = MyColor.Pristine;
                break;
            case RARITY.Legendary:
                SR.color = MyColor.Legendary;
                break;
            case RARITY.Mythic:
                SR.color = MyColor.Mythic;
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
