﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSheetController : MonoBehaviour{
    [HideInInspector]
    public Tab_0 Tab_0;
    [HideInInspector]
    public Tab_1 Tab_1;

    [HideInInspector]
    public MainPlayer MPC;

    int CachedTabIndex = 0;

    int CurrentTabIndex = 0;

    //[HideInInspector]
    public bool AllowControl = true;

    void Awake() {
        MPC = transform.parent.GetComponent<MainPlayerUI>().MPC;
        Tab_0 = transform.Find("Tab_0").GetComponent<Tab_0>();
        Tab_1 = transform.Find("Tab_1").GetComponent<Tab_1>();
    }

	void Start (){
        TurnOff();
    }

    void Update() {
        TabUpdate();
    }
    public void Toggle() {
        if (IsOn()) {
            TurnOff();
        } else {
            TurnOn();
        }

    }

    public void TurnOn() {
        ControllerManager.SyncActions = false;
        gameObject.SetActive(true);
        CurrentTabIndex = CachedTabIndex;
        switch (CachedTabIndex) {
            case 0:
                Tab_0.TurnOn();
                break;
            case 1:
                Tab_1.TurnOn();
                break;
        }
    }

    public void TurnOff() {
        if (!gameObject.active)
            return;
        CachedTabIndex = CurrentTabIndex;
        switch (CurrentTabIndex) {
            case 0:
                Tab_0.TurnOff();
                break;
            case 1:
                Tab_1.TurnOff();
                break;
        }        
        gameObject.SetActive(false);
        ControllerManager.SyncActions = true;
    }

    public bool IsOn() {
        return gameObject.active;
    }

    private void TabUpdate() {
        if (!AllowControl)
            return;
        if (ControllerManager.Actions.Flip.WasPressed) {
            switch (CurrentTabIndex) {
                case 0:
                    Tab_0.TurnOff();
                    Tab_1.TurnOn();
                    CurrentTabIndex = 1;
                    break;
                case 1:
                    Tab_1.TurnOff();
                    Tab_0.TurnOn();
                    CurrentTabIndex = 0;
                    break;
            }
        }
        else if (ControllerManager.Actions.PreviousPage.WasPressed) {
            switch (CurrentTabIndex) {
                case 0:
                    Tab_0.TurnOff();
                    Tab_1.TurnOn();
                    CurrentTabIndex = 1;
                    break;
                case 1:
                    Tab_1.TurnOff();
                    Tab_0.TurnOn();
                    CurrentTabIndex = 0;
                    break;
            }
        } else if (ControllerManager.Actions.NextPage.WasPressed) {
            switch (CurrentTabIndex) {
                case 0:
                    Tab_0.TurnOff();
                    Tab_1.TurnOn();
                    CurrentTabIndex = 1;
                    break;
                case 1:
                    Tab_1.TurnOff();
                    Tab_0.TurnOn();
                    CurrentTabIndex = 0;
                    break;
            }
        }
    }
}