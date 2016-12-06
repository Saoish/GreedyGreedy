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
        EI.Reset();
    }

    void OnEnable() {
        Slot = int.Parse(gameObject.name);
        MPC = transform.parent.parent.GetComponent<Tab_0>().MPC;
        EI = transform.parent.parent.Find("EquipmentInfo").GetComponent<EquipmentInfo>();
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        UpdateSlot();
        UpdateInfo();
    }

    public void OnClickEquip() {
        if (MPC.GetInventoryItem(Slot) != null) {
            if (MPC.Compatible(E)) {
                if (MPC.GetEquippedItem(E.EquipType) != null) {//Has Equipped Item
                    Equipment TakeOff = MPC.GetEquippedItem(E.EquipType);
                    MPC.UnEquip(TakeOff.EquipType);
                    MPC.Equip(E);
                    MPC.RemoveFromInventory(Slot, E);
                    MPC.AddToInventory(Slot, TakeOff);
                } else {//No Equipped Item
                    MPC.Equip(E);
                    MPC.RemoveFromInventory(Slot, E);
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
        if (MPC.GetInventoryItem(Slot) == E) {
            return;
        } else {
            E = MPC.GetInventoryItem(Slot);
            if (E != null) {
                if (EquipmentIcon != null)
                    DestroyObject(EquipmentIcon);
                EquipmentIcon = EquipmentController.ObtainInventoryIcon(E, transform);
            } else {
                DestroyObject(EquipmentIcon);
                EquipmentIcon = null;
            }
        }
    }
}