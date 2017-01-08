using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

public class EquippedButtonController : MonoBehaviour {
    MainPlayer MPC;
    GameObject EquipmentIcon;
    private EQUIPTYPE Slot;

    EquipmentInfo EI;

    Equipment E = null;

    public AudioClip selected;

    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
        EI.Show(E, EquipmentInfo.Mode.Equipped);
    }

    void OnEnable() {
        Slot = (EQUIPTYPE)int.Parse(gameObject.name);
        MPC = transform.parent.parent.GetComponent<Tab_0>().MPC;
        EI = transform.parent.parent.Find("EquipmentInfo").GetComponent<EquipmentInfo>();
        UpdateSlot();
    }

    void Update() {
        //UpdateSlot();
        //UpdateInfo();
    }
    void UpdateInfo() {
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject) {
            EI.Show(E, EquipmentInfo.Mode.Equipped);
        }
    }
    public void OnClickUnEquip() {
        if (!MPC.GetEquippedItem(Slot).isNull) {
            if (MPC.InventoryIsFull()) {//Inventory Full
                RedNotification.Push(RedNotification.Type.INVENTORY_FULL);
                return;
            } else {
                int SlotIndex = MPC.FirstAvailbleInventorySlot;
                MPC.AddToInventory(SlotIndex, E);
                transform.parent.parent.Find("InventoryButtons/" + SlotIndex).GetComponent<InventoryButtonController>().UpdateSlot();
                MPC.UnEquip(Slot);
                UpdateSlot();
                EI.Show(E, EquipmentInfo.Mode.Equipped);
            }
        }
    }
    public void UpdateSlot() {
        E = MPC.GetEquippedItem(Slot);
        if (!E.isNull) {
            if (EquipmentIcon != null)
                DestroyObject(EquipmentIcon);
            EquipmentIcon = EquipmentController.ObtainEquippedIcon(E, transform);
        } else {
            DestroyObject(EquipmentIcon);
            EquipmentIcon = null;
        }
        
    }
}