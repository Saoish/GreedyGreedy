using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Networking.Data;
using Networking;
using GreedyNameSpace;

public class ArenaInteractionContent : InteractionContent {    

    public GameObject CachedSelected;     

    private bool SyncActions = true;

    void Update() {
        if (SyncActions && ControllerManager.Actions.Cancel.WasPressed) {
            if (gameObject.active) {
                gameObject.SetActive(false);
                ControllerManager.SyncActions = true;                
            }
        }
    }

    protected override void OnEnable() {        
        ControllerManager.SyncActions = false;
        StartCoroutine(WaitForTinySecondsBeforeSelection(0.1f));
    }

    protected override void OnDisable() {    
        CachedSelected = EventSystem.current.currentSelectedGameObject;
        ControllerManager.SyncActions = true;        
    }    

    public void OneVOneOnClick() {                
        GetComponent<CanvasGroup>().interactable = false;
        SyncActions = false;
        PopUpNotification.Push("Do you want to queue up for " + MyText.Colofied(" Arena 1v1", "yellow") + "?", PopUpNotification.Type.Select);
        StartCoroutine(PopUpNotification.WaitForDecisionThenPerformAction(QueueUpArena,CancelRequest));
    }

    void QueueUpArena() {        
        Client.Send(Protocols.QueUpForArena_1v1);        
        GetComponent<CanvasGroup>().interactable = true;
        SyncActions = true;
    }

    void CancelRequest() {
        GetComponent<CanvasGroup>().interactable = true;
        SyncActions = true;
    }

    IEnumerator WaitForTinySecondsBeforeSelection(float time) {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForSeconds(time);        
        EventSystem.current.SetSelectedGameObject(CachedSelected);        
    }


}
