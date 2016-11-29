using UnityEngine;
using System.Collections;

public class Tab_0 : MonoBehaviour {
    [HideInInspector]
    public MainPlayer MPC;

    GameObject CachedButtonOJ = null;

    void Awake() {
        MPC = transform.parent.GetComponent<CharacterSheetController>().MPC;
    }

    void Start() {
    }


    void Update() {

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
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        if (CachedButtonOJ)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CachedButtonOJ);
        else {
            GameObject FBO = transform.Find("InventoryButtons/0").gameObject;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(FBO);
        }
    }

    public void TurnOff() {
        CachedButtonOJ = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        gameObject.SetActive(false);
    }

    public bool IsOn() {
        return gameObject.active;
    }
}
