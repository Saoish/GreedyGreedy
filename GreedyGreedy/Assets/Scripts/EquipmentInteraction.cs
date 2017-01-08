using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class EquipmentInteraction : Interaction {

    public override void Interact() {
        if (CacheManager.MP.InventoryIsFull())
            RedNotification.Push(RedNotification.Type.INVENTORY_FULL);
        else {
            CacheManager.MP.AddToInventory(CacheManager.MP.FirstAvailbleInventorySlot,GetComponentInParent<EquipmentController>().E);
            CacheManager.MP.InteractTarget = null;
            Destroy(transform.parent.gameObject);
        }
    }

    public override void Disengage() {
        CacheManager.MP.InteractTarget = null;
    }

    public override void SetMessage() {       
        InteractionNotification.Message = "Press " + MyText.Colofied(" X ", "blue") + " to pick up.";
    }

}
