using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

public class InventoryButtonController : MonoBehaviour {
    MainPlayer MPC;
    GameObject EquipmentIcon;
    EquipmentInfo EI;
    private int Slot = -999;


    Equipment E = null;

    public AudioClip selected;

    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
        EI.Show(E,EquipmentInfo.Mode.Inventory);        
    }

    void OnEnable() {      
        Slot = int.Parse(gameObject.name);
        MPC = transform.parent.parent.GetComponent<Tab_0>().MPC;
        EI = transform.parent.parent.Find("EquipmentInfo").GetComponent<EquipmentInfo>();
        UpdateSlot();        
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        //UpdateSlot();
        //UpdateInfo();
    }

    public void OnClickEquip() {
        if (!MPC.GetInventoryItem(Slot).isNull) {
            if (MPC.Compatible(E)) {
                if (!MPC.GetEquippedItem(E.EquipType).isNull) {//Has Equipped Item
                    Equipment TakeOff = MPC.GetEquippedItem(E.EquipType);
                    MPC.UnEquip(TakeOff.EquipType);
                    MPC.Equip(E);                    
                    transform.parent.parent.Find("EquippedSlotButtons/" + (int)E.EquipType).GetComponent<EquippedButtonController>().UpdateSlot();
                    MPC.RemoveFromInventory(Slot);
                    MPC.AddToInventory(Slot, TakeOff);
                    UpdateSlot();
                    EI.Show(E, EquipmentInfo.Mode.Inventory);
                } else {//No Equipped Item
                    MPC.Equip(E);
                    transform.parent.parent.Find("EquippedSlotButtons/" + (int)E.EquipType).GetComponent<EquippedButtonController>().UpdateSlot();
                    MPC.RemoveFromInventory(Slot);                    
                    UpdateSlot();
                    EI.Show(E, EquipmentInfo.Mode.Inventory);
                }
            } else {
                RedNotification.Push(RedNotification.Type.CANT_EQUIP);
            }
        }
    }

    void UpdateInfo() {
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject) {
            EI.Show(E, EquipmentInfo.Mode.Inventory);
        }
    }

    public void UpdateSlot() {        
        E = MPC.GetInventoryItem(Slot);
        if (!E.isNull) {
            if (EquipmentIcon != null)
                DestroyObject(EquipmentIcon);
            EquipmentIcon = EquipmentController.ObtainInventoryIcon(E, transform);
        } else {
            DestroyObject(EquipmentIcon);
            EquipmentIcon = null;
        }        
    }
}