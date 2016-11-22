using UnityEngine;
using System.Collections;

public class Tab_0 : MonoBehaviour {

    private PlayerController PC;

    GameObject ES;

    GameObject CachedButtonOJ = null;

    void Awake() {
        ES = GameObject.Find("EventSystem");
        PC = transform.parent.GetComponent<CharacterSheetController>().PC;
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
        ES.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        if (CachedButtonOJ)
            ES.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(CachedButtonOJ);
        else {
            GameObject FBO = transform.Find("InventoryButtons/0").gameObject;
            ES.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(FBO);
        }
    }

    public void TurnOff() {
        if(CachedButtonOJ)
            CachedButtonOJ =  ES.GetComponent<UnityEngine.EventSystems.EventSystem>().currentSelectedGameObject;
        gameObject.SetActive(false);
    }

    public bool IsOn() {
        //ES.GetComponent<UnityEngine.EventSystems.EventSystem>().i
        return gameObject.active;
    }
}
