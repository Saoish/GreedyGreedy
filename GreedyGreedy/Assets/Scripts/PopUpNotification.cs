using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Networking;
using System;

public class PopUpNotification : MonoBehaviour {    
    public enum Type {
        Pending,
        Confirm,
        Select
    }

    public static bool Decision = false;
    static bool Decided = false;
    public static bool SyncActions = false;

    static GameObject OK_BtnOJ;
    static GameObject Yes_BtnOJ;
    static GameObject No_BtnOJ;

    static Text msg;
    static Animator anim;
    static GameObject self;

    static GameObject CachedPointer;

    //static CanvasGroup[] cgs;   

    void Awake() {
        //cgs = transform.parent.GetComponentsInChildren<CanvasGroup>();
        msg = transform.Find("Message").GetComponent<Text>();
        anim = GetComponent<Animator>();
        self = gameObject;
        OK_BtnOJ = transform.Find("Buttons/OK").gameObject;
        Yes_BtnOJ = transform.Find("Buttons/Yes").gameObject;
        No_BtnOJ = transform.Find("Buttons/No").gameObject;
        self.SetActive(false);
    }


    void Update() {
        if (SyncActions && ControllerManager.Actions.Cancel.WasPressed)
            Cancel();        
    }


    public static void Push(string message,Type type = Type.Pending) {       
        DisableAllCanvasGroup();
        msg.text = message;
        self.SetActive(true);
        anim.Play("popup", 0, 0);
        switch (type) {
            case Type.Pending:
                break;
            case Type.Confirm:
                Yes_BtnOJ.SetActive(false);
                No_BtnOJ.SetActive(false);
                OK_BtnOJ.SetActive(true);
                CachedPointer = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(OK_BtnOJ);
                break;
            case Type.Select:
                OK_BtnOJ.SetActive(false);
                Yes_BtnOJ.SetActive(true);
                No_BtnOJ.SetActive(true);
                CachedPointer = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(No_BtnOJ);
                SyncActions = true;
                break;
        }

    }

    //public static void TurnOffAllInteractionWindow() {
    //    foreach(InteractionContent IC in FindObjectsOfType<InteractionContent>()) {
    //        IC.TurnOff();
    //    }
    //}

    static private void DisableAllCanvasGroup() {
        foreach (CanvasGroup cg in FindObjectsOfType<CanvasGroup>()) {
            cg.interactable = false;
        }
    }

    static private void EnableAllCanvasGroup() {
        foreach (CanvasGroup cg in FindObjectsOfType<CanvasGroup>()) {
            cg.interactable = true;
        }
    }

    private void Cancel() {
        SyncActions = false;
        Decided = true;
        Decision = false;
        gameObject.SetActive(false);
        EnableAllCanvasGroup();
        if (CachedPointer != null) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CachedPointer);
            CachedPointer = null;
        }        
    }

    public void Confirm() {
        OK_BtnOJ.SetActive(false);
        gameObject.SetActive(false);
        EnableAllCanvasGroup();
        if (CachedPointer != null) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CachedPointer);
            CachedPointer = null;
        }
    }

    public void Yes() {
        SyncActions = false;
        Decided = true;
        Decision = true;
        Yes_BtnOJ.SetActive(false);
        No_BtnOJ.SetActive(false);
        gameObject.SetActive(false);
        EnableAllCanvasGroup();
        if (CachedPointer != null) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CachedPointer);
            CachedPointer = null;
        }        
    }

    public void No() {
        SyncActions = false;
        Decided = true;
        Decision = false;
        Yes_BtnOJ.SetActive(false);
        No_BtnOJ.SetActive(false);
        gameObject.SetActive(false);
        EnableAllCanvasGroup();
        if (CachedPointer != null) {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CachedPointer);
            CachedPointer = null;
        }
    }

    public static IEnumerator WaitForDecisionThenPerformAction<T>(Action<T> Call,T para) {
        Decided = false;
        Decision = false;
        while (Decided == false)
            yield return null;
        if(Decision)
            Call(para);
    }
    
    public static IEnumerator WaitForDecisionThenPerformAction(Action ConfirmCall,Action DenyCall = null) {
        Decided = false;
        Decision = false;
        while (Decided == false)
            yield return null;
        if (Decision)
            ConfirmCall();
        else {
            if (DenyCall != null)
                DenyCall();
        }
    }

    public static IEnumerator WaitForDecision() {
        Decided = false;
        Decision = false;
        while (Decided == false)
            yield return null;
    }    
}
