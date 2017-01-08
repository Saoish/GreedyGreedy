using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GreedyNameSpace;
using InControl;
using System.Collections.Generic;

public class ActiveSkillButtonController : MonoBehaviour {
    MainPlayer MPC;
    private int Slot = -999;

    [HideInInspector]
    public ActiveSkill ActiveSkill;    

    private PlayerAction Key;

    private Image IconImage;
    private Image CD_Mask;
    private GameObject Red_Mask_OJ;
    private Transform BG;

    private List<ActiveSkillButtonController> OtherASBCs;


    bool Assigning = false;

    public AudioClip selected;


    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
    }

    void Awake() {
        OtherASBCs = new List<ActiveSkillButtonController>();
    }

    // Use this for initialization
    void Start() {
        Slot = int.Parse(gameObject.name);
        IconImage = GetComponent<Image>();
        CD_Mask = transform.Find("CD_Mask").GetComponent<Image>();
        Red_Mask_OJ = transform.Find("Red_Mask").gameObject;
        BG = transform.parent;
        GetComponent<Button>().onClick.AddListener(OnClickActive);

        MPC = BG.parent.parent.parent.GetComponent<MainPlayerUI>().MPC;

        MapKey();
        FetchSkill();        

        foreach (ActiveSkillButtonController ASBC in transform.parent.parent.GetComponentsInChildren<ActiveSkillButtonController>()) {
            if (ASBC != GetComponent<ActiveSkillButtonController>()) {
                OtherASBCs.Add(ASBC);
            } 
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (ActiveSkill) {
            UpdateCDMask();
            UpdateRedMask();
        }
        if (!ControllerManager.SyncActions && !Assigning) {
            GetComponent<Button>().interactable = false;
            if(ActiveSkill)
                ActiveSkill.Interrupt();
            return;
        } else if (Assigning) {
            AssigningUpdate();
        } else if(ControllerManager.SyncActions){
            CombatUpdate();
        }
    }

    public void FetchSkill() {
        ActiveSkill = MPC.GetActiveSlotSkill(Slot);
        LoadSkillIcon();
    }

    public void DiscardSkill() {
        MPC.SetActiveSkillAt(Slot, null);
        FetchSkill();
    }


    void OnClickAssign(SkillButton skill_button) {
        ActiveSkill active_skill = (ActiveSkill)skill_button.Skill;
        if(ActiveSkill!=null && ActiveSkill.RealTime_CD != 0) {
            RedNotification.Push(RedNotification.Type.ON_CD);
            return;
        }
        foreach (ActiveSkillButtonController ASBC in OtherASBCs) {
            if (ASBC.ActiveSkill != null && ASBC.ActiveSkill.Name == active_skill.Name) {
                if (ASBC.ActiveSkill.RealTime_CD != 0) {
                    RedNotification.Push(RedNotification.Type.ON_CD);
                    return;
                }
                ASBC.DiscardSkill();
            }
        }
        MPC.SetActiveSkillAt(Slot, active_skill);
        FetchSkill();
        DisableActiveSlotsAssigning();
        skill_button.SkillSubMenu.TurnOff();
    }

    void DisableActiveSlotsAssigning() {
        DisableAssigning();
        foreach (ActiveSkillButtonController ASBC in OtherASBCs) {
            ASBC.DisableAssigning();            
        }
    }

    public void EnableAssigning(SkillButton skill_button) {
        Assigning = true;
        transform.parent.GetComponent<Animator>().SetBool("Blinking", true);
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate { OnClickAssign(skill_button); });
        Navigation auto = new Navigation();
        auto.mode = Navigation.Mode.Automatic;
        transform.GetComponent<Button>().navigation = auto;
    }

    public void DisableAssigning() {
        Assigning = false;
        transform.parent.GetComponent<Animator>().SetBool("Blinking", false);
        GetComponent<Button>().interactable = false;
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClickActive);
        Navigation none = new Navigation();
        none.mode = Navigation.Mode.None;
        transform.GetComponent<Button>().navigation = none;
    }


    void MapKey() {
        switch (Slot) {
            case 0:
                Key = ControllerManager.Actions.Skill_0;
                break;
            case 1:
                Key = ControllerManager.Actions.Skill_1;
                break;
            case 2:
                Key = ControllerManager.Actions.Skill_2;
                break;
            case 3:
                Key = ControllerManager.Actions.Skill_3;
                break;
        }
    }

    void OnClickActive() {
        if (!ActiveSkill) {
            return;
        }
        if (ActiveSkill.Ready()) {
            InterruptOthers();
            switch (ActiveSkill.type) {
                case ActiveSkill.Type.Instant:
                    ActiveSkill.Active();
                    break;
                case ActiveSkill.Type.Manual:                    
                    ActiveSkill.ActiveIndicator();
                    break;
            }            
        }
    }

    void UpdateRedMask() {
        if (MPC.GetCurrStats(STATSTYPE.ESSENSE) - ActiveSkill.EssenseCost < 0) {
            Red_Mask_OJ.SetActive(true);
        } else
            Red_Mask_OJ.SetActive(false);
    }

    void UpdateCDMask() {
        if (ActiveSkill.RealTime_CD - Time.deltaTime <= 0 && ActiveSkill.RealTime_CD != 0) {
            BG.GetComponent<Animator>().Play("bg_blink");
        }
        CD_Mask.fillAmount = ActiveSkill.GetCDPortion();
    }

    void LoadSkillIcon() {
        if (!ActiveSkill) {
            IconImage.sprite = null;
            Color ImageIconColor = IconImage.color;
            ImageIconColor.a = 0;
            IconImage.color = ImageIconColor;
        }
        else {
            IconImage.sprite = Resources.Load<Sprite>("SkillIcons/"+ActiveSkill.Name);
            Color ImageIconColor = IconImage.color;
            ImageIconColor.a = 255;
            IconImage.color = ImageIconColor;
        }
    }

    void AssigningUpdate() {
        GetComponent<Button>().interactable = true;
        var pointer = new PointerEventData(EventSystem.current);
        if (Key.WasPressed) {
            ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject)
            transform.parent.GetComponent<Image>().color = MyColor.Green;
        else
            transform.parent.GetComponent<Image>().color = MyColor.Yellow;
    }

    void CombatUpdate() {
        if (!MPC.Alive)
            return;
        GetComponent<Button>().interactable = true;
        if (ActiveSkill == null)
            return;
        var pointer = new PointerEventData(EventSystem.current);
        if (Key.WasPressed)
            ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        else if (Key.WasReleased && ActiveSkill.Indicating) {
            if (ActiveSkill.type == ActiveSkill.Type.Manual && ActiveSkill.Ready()) {
                ActiveSkill.Active();
            }
        }
        else if(ControllerManager.Actions.Cancel.WasPressed && ActiveSkill.Indicating) {
            ActiveSkill.Interrupt();
        }
    }

    void InterruptOthers() {
        foreach(ActiveSkillButtonController ASBC in OtherASBCs) {
            if(ASBC.ActiveSkill!=null)
                ASBC.ActiveSkill.Interrupt();
        }
    }
}
