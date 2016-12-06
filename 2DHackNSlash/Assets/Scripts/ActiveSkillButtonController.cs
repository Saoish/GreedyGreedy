using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GreedyNameSpace;

public class ActiveSkillButtonController : MonoBehaviour {
    MainPlayer MPC;
    private int Slot = -999;

    [HideInInspector]
    public ActiveSkill ActiveSkill;

    private KeyCode K_Key;
    private string J_Key;

    private Image IconImage;
    private Image CD_Mask;
    private GameObject Red_Mask_OJ;
    private Transform BG;

    bool Assigning = false;

    ActiveSkillButtonController[] ASBCs;

    public AudioClip selected;

    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
    }

    void Awake() {

    }
	// Use this for initialization
	void Start () {
        Slot = int.Parse(gameObject.name);
        IconImage = GetComponent<Image>();
        CD_Mask = transform.Find("CD_Mask").GetComponent<Image>();
        Red_Mask_OJ = transform.Find("Red_Mask").gameObject;
        BG = transform.parent;
        GetComponent<Button>().onClick.AddListener(OnClickActive);

        MPC = BG.parent.parent.parent.GetComponent<MainPlayerUI>().MPC;

        ASBCs = transform.parent.parent.GetComponentsInChildren<ActiveSkillButtonController>();

        MapKey();
        FetchSkill();
    }
	
	// Update is called once per frame
	void Update () {
        if (ActiveSkill) {
            UpdateCDMask();
            UpdateRedMask();
        }
        if (!ControllerManager.AllowControlUpdate && !Assigning) {
            GetComponent<Button>().interactable = false;
            return;
        }
        GetComponent<Button>().interactable = true;
        var pointer = new PointerEventData(EventSystem.current);
        if (J_Key == ControllerManager.J_LTRT && K_Key == ControllerManager.Skill2) {//2
            if (Input.GetKeyDown(K_Key) || Input.GetAxisRaw(J_Key) > 0)
                ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        } else if (J_Key == ControllerManager.J_LTRT && K_Key == ControllerManager.Skill3) {//3
            if (Input.GetKeyDown(K_Key) || Input.GetAxisRaw(J_Key) < 0)
                ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        } else if (Input.GetKeyDown(K_Key) || Input.GetKeyDown(J_Key)) {
            ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        AssigningUpdate();
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
        foreach (ActiveSkillButtonController ASBC in ASBCs) {//Prevent Duplicated slot
            if (ASBC != GetComponent<ActiveSkillButtonController>() && ASBC.ActiveSkill != null && ASBC.ActiveSkill.SD.Name == active_skill.SD.Name) {
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
        foreach (ActiveSkillButtonController ASBC in ASBCs) {
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
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClickActive);
        Navigation none = new Navigation();
        none.mode = Navigation.Mode.None;
        transform.GetComponent<Button>().navigation = none;
    }


    void MapKey() {
        switch (Slot) {
            case 0:
                K_Key = ControllerManager.Skill0;
                J_Key = ControllerManager.J_LB;
                break;
            case 1:
                K_Key = ControllerManager.Skill1;
                J_Key = ControllerManager.J_RB;
                break;
            case 2:
                K_Key = ControllerManager.Skill2;
                J_Key = ControllerManager.J_LTRT;
                break;
            case 3:
                K_Key = ControllerManager.Skill3;
                J_Key = ControllerManager.J_LTRT;
                break;
        }
    }

    void OnClickActive() {
        if (!ActiveSkill) {
            return;
        }
        if (ActiveSkill.Ready()) {
            ActiveSkill.Active();
        }
    }

    void UpdateRedMask() {
        if (MPC.GetCurrStats(StatsType.MANA) - ActiveSkill.ManaCost < 0) {
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
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject)
            transform.parent.GetComponent<Image>().color = MyColor.Green;
        else
            transform.parent.GetComponent<Image>().color = MyColor.Yellow;
    }
}
