using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSheetController : MonoBehaviour{
    Tab_0 Tab_0;
    Tab_1 Tab_1;

    public PlayerController PC;

    ControllerManager CM;

    int CachedTabIndex = 0;

    int CurrentTabIndex = 0;

    void Awake() {
        PC = transform.parent.GetComponent<PlayerUIController>().PC;
        CM = PC.GetCM();
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
    }

    public bool IsOn() {
        return gameObject.active;
    }

    private void TabUpdate() {
        if (Input.GetKeyDown(CM.Tab)) {
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
        else if (Input.GetKeyDown(CM.J_LB)) {
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
        } else if (Input.GetKeyDown(CM.J_RB)) {
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
