using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tab_1 : MonoBehaviour {

    private PlayerController PC;

    GameObject ES;

    GameObject CachedButtonOJ = null;

    public Button SkillButton_0;
    public Button SkillButton_1;
    public Button SkillButton_2;
    public Button SkillButton_3;

    void Awake() {
        ES = GameObject.Find("EventSystem");
        PC = transform.parent.GetComponent<CharacterSheetController>().PC;

    }

    void Start() {

    }


    void Update() {
        SkillPointsUpdate();
    }

    void SkillPointsUpdate() {
        transform.Find("Skill Points").GetComponent<Text>().text = "Skill Points: "+PC.GetPlayerData().SkillPoints.ToString();
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
            GameObject FBO = SkillButton_0.transform.gameObject;
            ES.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(FBO);
        }

    }

    public void TurnOff() {
        CachedButtonOJ = ES.GetComponent<UnityEngine.EventSystems.EventSystem>().currentSelectedGameObject;
        gameObject.SetActive(false);
    }

    public bool IsOn() {
        return gameObject.active;
    }

    void EnableSkillButtonsSelection() {
        Navigation auto = new Navigation();
        auto.mode = Navigation.Mode.Automatic;
        SkillButton_0.navigation = auto;
        SkillButton_1.navigation = auto;
        SkillButton_2.navigation = auto;
        SkillButton_3.navigation = auto;
    }

    void DisableSkillButtonsSelection() {
        Navigation none = new Navigation();
        none.mode = Navigation.Mode.None;
        SkillButton_0.navigation = none;
        SkillButton_1.navigation = none;
        SkillButton_2.navigation = none;
        SkillButton_3.navigation = none;
    }
}

