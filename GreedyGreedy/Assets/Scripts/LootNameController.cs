using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

public class LootNameController : MonoBehaviour {
    Equipment E;
    void Start() {
        E = GetComponentInParent<EquipmentController>().E;
        Text t = GetComponent<Text>();
        switch (E.Rarity) {
            case RARITY.Common:
                t.color = MyColor.Common;
                break;
            case RARITY.Fine:
                t.color = MyColor.Fine;
                break;
            case RARITY.Pristine:
                t.color = MyColor.Pristine;
                break;
            case RARITY.Legendary:
                t.color = MyColor.Legendary;
                break;
            case RARITY.Mythic:
                t.color = MyColor.Mythic;
                break;
        }
        t.text = E.Name;        
    }
    
    public void TurnOn() {
        gameObject.SetActive(true);
    }
    public void TurnOff() {
        gameObject.SetActive(false);
    }

    public void ShowNameUpdate() {
        if (GameManager.Show_Names == -1) {
            TurnOff();
        } else
            TurnOn();
    }
}