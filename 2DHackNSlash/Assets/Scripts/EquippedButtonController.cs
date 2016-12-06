using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

public class EquippedButtonController : MonoBehaviour {
    MainPlayer MPC;
    GameObject EquipmentIcon;
    private EquipType Slot;

    EquipmentInfo EI;

    Equipment E = null;

    public AudioClip selected;

    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
        EI.Reset();
    }

    void OnEnable() {
        Slot = (EquipType)int.Parse(gameObject.name);
        MPC = transform.parent.parent.GetComponent<Tab_0>().MPC;
        EI = transform.parent.parent.Find("EquipmentInfo").GetComponent<EquipmentInfo>();
        UpdateSlot();
    }

    void Update() {
        UpdateSlot();
        UpdateInfo();
    }
    void UpdateInfo() {
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject) {
            EI.Show(E, EquipmentInfo.Mode.Equipped);
        }
    }
    public void OnClickUnEquip() {
        if (MPC.GetEquippedItem(Slot) != null) {
            if (MPC.InventoryIsFull()) {//Inventory Full
                RedNotification.Push(RedNotification.Type.INVENTORY_FULL);
                return;
            } else {
                int SlotIndex = MPC.FirstAvailbleInventorySlot();
                MPC.AddToInventory(SlotIndex, E);
                MPC.UnEquip(Slot);
            }
        }
    }
    public void UpdateSlot() {
        if (MPC.GetEquippedItem(Slot) == E) {
            return;
        } else {
            E = MPC.GetEquippedItem(Slot);
            if (E != null) {
                if (EquipmentIcon != null)
                    DestroyObject(EquipmentIcon);
                EquipmentIcon = EquipmentController.ObtainEquippedIcon(E, transform);
            } else {
                DestroyObject(EquipmentIcon);
                EquipmentIcon = null;
            }
        }
    }
}