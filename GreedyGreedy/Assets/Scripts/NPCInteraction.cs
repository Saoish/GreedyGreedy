using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class NPCInteraction : Interaction {
    public InteractionContent IC;

    public override void Interact() {        
        IC.TurnOn();
    }

    public override void Disengage() {
        CacheManager.MP.InteractTarget = null;
    }

    public override void SetMessage() {        
        InteractionNotification.Message = "Press " + MyText.Colofied(" X ", "blue") + " to talk.";
    }
}
