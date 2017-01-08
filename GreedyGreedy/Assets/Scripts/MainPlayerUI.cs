using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

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
    public bool SyncActions = true;

	void Awake () {
        MPC = transform.parent.GetComponent<MainPlayer>();
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
        if (!SyncActions)
            return;
        if (ControllerManager.SyncActions) {
            if (ControllerManager.Actions.Menu.WasPressed)
                MC.TurnOn();
            else if (ControllerManager.Actions.Inventory.WasPressed)
                CSC.TurnOn();
        }
        if (ControllerManager.Actions.Cancel.WasPressed) {
            CSC.TurnOff();
            MC.TurnOff();
        }
    }

    public void UpdateHealthManaBar() {
        if(MPC.GetCurrStats(STATSTYPE.HEALTH)/MPC.GetMaxStats(STATSTYPE.HEALTH) >=0)
            HealthMask.transform.localScale = new Vector2(1, MPC.GetCurrStats(STATSTYPE.HEALTH) / MPC.GetMaxStats(STATSTYPE.HEALTH));
        else
            HealthMask.transform.localScale = new Vector2(1, 0);
        if (MPC.GetCurrStats(STATSTYPE.ESSENSE) / MPC.GetMaxStats(STATSTYPE.ESSENSE) >= 0)
            ManaMask.transform.localScale = new Vector2( 1, MPC.GetCurrStats(STATSTYPE.ESSENSE) / MPC.GetMaxStats(STATSTYPE.ESSENSE));
        else
            ManaMask.transform.localScale = new Vector2(1,0);
    }

    public void UpdateExpBar() {
        ExpMask.GetComponent<Image>().fillAmount = ((float)MPC.GetExp() / (float)MPC.GetNextLvlExp());
    }


    public void UpdateEquippedSlot(EQUIPTYPE slot) {
        CSC.Tab_0.transform.Find("EquippedSlotButtons/" + (int)slot).GetComponent<EquippedButtonController>().UpdateSlot();
    }

    public void UpdateInventorySlot(int slot) {
        CSC.Tab_0.transform.Find("InventoryButtons/" + slot).GetComponent<InventoryButtonController>().UpdateSlot();
    }
}
