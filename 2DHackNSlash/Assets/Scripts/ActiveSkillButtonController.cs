using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActiveSkillButtonController : MonoBehaviour {
    ControllerManager CM;
    PlayerController PC;
    private int Slot = -999;

    private ActiveSkill Skill;

    private KeyCode K_Key;
    private string J_Key;

    private Image IconImage;
    private Image CD_Mask;
    private GameObject Red_Mask_OJ;
    private Transform BG;


    void MapKey() {
        switch (Slot) {
            case 0:
                K_Key = CM.Skill0;
                J_Key = CM.J_LB;
                break;
            case 1:
                K_Key = CM.Skill1;
                J_Key = CM.J_RB;
                break;
            case 2:
                K_Key = CM.Skill2;
                J_Key = CM.J_LTRT;
                break;
            case 3:
                K_Key = CM.Skill3;
                J_Key = CM.J_LTRT;
                break;
        }
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
        GetComponent<Button>().onClick.AddListener(ActiveSkill);
        PC = GameObject.Find("MainPlayer/PlayerController").transform.GetComponent<PlayerController>();
        CM = PC.GetCM();
        MapKey();
        Skill = PC.GetActiveSlotSkillTransform(Slot);
        LoadSkillIcon();
    }
	
	// Update is called once per frame
	void Update () {
        if (!CM.AllowControlUpdate) {
            GetComponent<Button>().interactable = false;
            return;
        }
        GetComponent<Button>().interactable = true;
        var pointer = new PointerEventData(EventSystem.current);
        if (J_Key == CM.J_LTRT && K_Key == CM.Skill2) {//2
            if (Input.GetKeyDown(K_Key) || Input.GetAxisRaw(J_Key) > 0)
                ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        } else if (J_Key == CM.J_LTRT && K_Key == CM.Skill3) {//3
            if (Input.GetKeyDown(K_Key) || Input.GetAxisRaw(J_Key) < 0)
                ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        } else if (Input.GetKeyDown(K_Key) || Input.GetKeyDown(J_Key)) {
            ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        if (Skill) {
            UpdateCDMask();
            UpdateRedMask();
        }

    }

    void ActiveSkill() {
        if (Skill.Ready()) {
            Skill.Active();
        }
    }

    void UpdateRedMask() {
        if (PC.GetCurrMana() - Skill.ManaCost < 0) {
            Red_Mask_OJ.SetActive(true);
        } else
            Red_Mask_OJ.SetActive(false);
    }

    void UpdateCDMask() {
        if (Skill.RealTime_CD - Time.deltaTime <= 0 && Skill.RealTime_CD != 0) {
            BG.GetComponent<Animator>().Play("bg_blank");
        }
        CD_Mask.fillAmount = Skill.GetCDPortion();
    }

    void LoadSkillIcon() {
        if (!Skill) {
            IconImage.sprite = null;
            Color ImageIconColor = IconImage.color;
            ImageIconColor.a = 0;
            IconImage.color = ImageIconColor;
        }
        if (Skill != null) {
            IconImage.sprite = Resources.Load<Sprite>("SkillIcons/"+Skill.SD.Name);
            Color ImageIconColor = IconImage.color;
            ImageIconColor.a = 255;
            IconImage.color = ImageIconColor;
        }
    }
}
