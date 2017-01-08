using UnityEngine;
using System.Collections;

public class Tab_0 : MonoBehaviour {
    [HideInInspector]
    public MainPlayer MPC;

    public GameObject CachedButtonOJ;

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
        if (gameObject.active)
            return;
        gameObject.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);        
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CachedButtonOJ);
    }

    public void TurnOff() {
        if (!gameObject.active)
            return;        
        CachedButtonOJ = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        gameObject.SetActive(false);        
    }

    public bool IsOn() {
        return gameObject.active;
    }
}
