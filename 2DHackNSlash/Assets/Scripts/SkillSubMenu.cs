using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillSubMenu : MonoBehaviour {

    GameObject LvlUpBtn_OJ;
    GameObject AssignBtn_OJ;

    void Awake() {
        LvlUpBtn_OJ = transform.Find("Level Up").gameObject;
        AssignBtn_OJ = transform.Find("Assign").gameObject;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(ControllerManager.ESC) || Input.GetKeyDown(ControllerManager.J_B)) {
            transform.parent.GetComponent<SkillButton>().DisableActiveSlotsAssigning();
            TurnOff();
        }
    }


    public bool IsOn() {
        return gameObject.active;
    }

    public void TurnOn() {
        transform.parent.GetComponent<SkillButton>().MPUI.AllowControl = false;
        transform.parent.GetComponent<SkillButton>().CSC.AllowControl = false;
        gameObject.SetActive(true);
        DisableAllSkillButtonNavigation();
        if (transform.parent.GetComponent<SkillButton>().Skill.GetType().IsSubclassOf(typeof(ActiveSkill))) {
            AssignBtn_OJ.SetActive(true);
        }
        LvlUpBtn_OJ.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(LvlUpBtn_OJ);
    }

    public void TurnOff() {
        EnableAllSkillButtonNavigation();
        EnableSubmenuNavigation();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(transform.parent.gameObject);
        gameObject.SetActive(false);
        transform.parent.GetComponent<SkillButton>().RestoreCSCMPUIControl();
    }

    public void DisableAllSkillButtonNavigation() {
        Navigation none = new Navigation();
        none.mode = Navigation.Mode.None;
        foreach (SkillButton SB in transform.parent.parent.parent.parent.GetComponentsInChildren<SkillButton>())
            SB.transform.GetComponent<Button>().navigation = none;
    }

    public void EnableAllSkillButtonNavigation() {
        Navigation auto = new Navigation();
        auto.mode = Navigation.Mode.Automatic;
        foreach (SkillButton SB in transform.parent.parent.parent.parent.GetComponentsInChildren<SkillButton>())
            SB.transform.GetComponent<Button>().navigation = auto;
    }
    
    public void DisableSubmenuNavigation() {
        Navigation none = new Navigation();
        none.mode = Navigation.Mode.None;
        LvlUpBtn_OJ.transform.GetComponent<Button>().navigation = none;
        AssignBtn_OJ.transform.GetComponent<Button>().navigation = none;
    }

    public void EnableSubmenuNavigation() {
        Navigation auto = new Navigation();
        auto.mode = Navigation.Mode.Automatic;
        LvlUpBtn_OJ.transform.GetComponent<Button>().navigation = auto;
        AssignBtn_OJ.transform.GetComponent<Button>().navigation = auto;
    }
}
