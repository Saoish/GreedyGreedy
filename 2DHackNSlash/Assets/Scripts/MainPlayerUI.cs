using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainPlayerUI : MonoBehaviour {
    [HideInInspector]
    public MainPlayer MPC;

    Transform HealthMask;
    Transform ManaMask;
    Transform ExpMask;

    MenuController MC;
    CharacterSheetController CSC;

    GameObject ES;

    [HideInInspector]
    public bool AllowControl = true;

	void Awake () {
        MPC = transform.parent.Find("PlayerController").GetComponent<MainPlayer>();
        HealthMask = transform.Find("Action Bar/Health Orb");
        ManaMask = transform.Find("Action Bar/Mana Orb");
        ExpMask = transform.Find("Action Bar/XP mask");

        MC = transform.Find("Menu").GetComponent<MenuController>();
        CSC = transform.Find("CharacterSheet").GetComponent<CharacterSheetController>();
    }

    void Start() {

    }
	
	// Update is called once per frame
	void Update () {
        PopUpUIUpdate();
        UpdateHealthManaBar();
        UpdateExpBar();
    }

    private void PopUpUIUpdate() {
        if (!AllowControl)
            return;
        if (Input.GetKeyDown(ControllerManager.ESC) || Input.GetKeyDown(ControllerManager.J_Start)) {
            ControllerManager.MoveVector = Vector2.zero;
            ControllerManager.AttackVector = Vector2.zero;
            if (CSC.IsOn())
                CSC.TurnOff();
            else
                MC.Toggle();

        } else if (Input.GetKeyDown(ControllerManager.CharacterSheet) || Input.GetKeyDown(ControllerManager.J_X)) {
            ControllerManager.MoveVector = Vector2.zero;
            ControllerManager.AttackVector = Vector2.zero;
            if (MC.IsOn())
                MC.TurnOff();
            CSC.Toggle();
        } else if (Input.GetKeyDown(ControllerManager.J_B)) {
            if (CSC.IsOn())
                CSC.TurnOff();
            if (MC.IsOn())
                MC.TurnOff();
        }
        if (MC.IsOn() || CSC.IsOn())
            ControllerManager.AllowControlUpdate = false;
        else
            ControllerManager.AllowControlUpdate = true;
    }

    public void UpdateHealthManaBar() {
        if(MPC.GetCurrHealth()/MPC.GetMaxHealth()>=0)
            HealthMask.transform.localScale = new Vector2(1, MPC.CurrHealth / MPC.GetMaxHealth());
        else
            HealthMask.transform.localScale = new Vector2(1, 0);
        if (MPC.CurrMana / MPC.GetMaxMana() >= 0)
            ManaMask.transform.localScale = new Vector2( 1, MPC.CurrMana / MPC.GetMaxMana());
        else
            ManaMask.transform.localScale = new Vector2(1,0);
    }

    public void UpdateExpBar() {
        ExpMask.GetComponent<Image>().fillAmount = ((float)MPC.GetExp() / (float)MPC.GetNextLvlExp());
    }

}
