﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GreedyNameSpace;

public class SkillButton : MonoBehaviour {
    //public GameObject SkillInfo;
    SkillInfo SI;

    [HideInInspector]
    public MainPlayer MPC;

    [HideInInspector]
    public int SkillIndex = -999;

    [HideInInspector]
    public Skill Skill;

    private Image IconImage;

    [HideInInspector]
    public SkillSubMenu SkillSubMenu;

    string PathName;

    int Tier1_Req = Patch.Tier1_Req;
    int Tier2_Req = Patch.Tier2_Req;
    int Tier3_Req = Patch.Tier3_Req;

    List<int> L_Tier1 = new List<int>() { 0, 1, 2 };
    List<int> L_Tier2 = new List<int>() { 3, 4, 5 };
    List<int> L_Tier3 = new List<int>() { 6, 7, 8 };

    List<int> R_Tier1 = new List<int>() { 9, 10, 11 };
    List<int> R_Tier2 = new List<int>() { 12, 13, 14 };
    List<int> R_Tier3 = new List<int>() { 15, 16, 17 };

    enum SkillPosition { LT1, LT2, LT3, RT1, RT2, RT3 };

    SkillPosition SP;

    [HideInInspector]
    public MainPlayerUI MPUI;
    [HideInInspector]
    public CharacterSheetController CSC;

    ActiveSkillButtonController[] ASBCs;

    public AudioClip skill_lvlup;

    public AudioClip selected;

    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
    }

    void Awake() {
    }

    // Use this for initialization
    void Start() {
        SkillIndex = int.Parse(gameObject.name);
        IconImage = GetComponent<Image>();
        MPC = transform.parent.parent.parent.GetComponent<Tab_1>().MPC;
        MPUI = MPC.transform.Find("MainPlayerUI").GetComponent<MainPlayerUI>();
        CSC = MPUI.transform.Find("CharacterSheet").GetComponent<CharacterSheetController>();
        FetchSkill();
        LoadSkillIcon();
        SI = transform.parent.parent.parent.Find("SkillInfo").GetComponent<SkillInfo>();
        SkillSubMenu = transform.Find("Skill Sub Menu").GetComponent<SkillSubMenu>();
        AssignSkillPosition();
        FetchPathName();
        ASBCs = MPUI.transform.Find("Action Bar/Active Skill Panel").GetComponentsInChildren<ActiveSkillButtonController>();
    }

    // Update is called once per frame
    void Update() {
        SkillInfoUpdate();

    }

    public void ActiveSubMenu() {
        SkillSubMenu.TurnOn();
    }

    public void LvlUp() {
        if (MPC.GetAvailableSkillPoints() <= 0) {
            RedNotification.Push(RedNotification.Type.NO_SKILL_POINT);
        } else if (!MeetRequirement())
            RedNotification.Push(RedNotification.Type.SKILL_REQUIREMENT_NOT_MET);
        else if (Skill!=null && Skill.lvl == 5) {
            RedNotification.Push(RedNotification.Type.MAX_SKILL_LVL);
        } else {
            MPC.LvlUpSkill(SkillIndex);
            FetchSkill();
            FetechActiveSlotSkills();
            SkillSubMenu.TurnOff();
            AudioSource.PlayClipAtPoint(skill_lvlup, transform.position, GameManager.SFX_Volume);
        }
    }

    public void Assign() {
        if (Skill.lvl<=0)
            RedNotification.Push(RedNotification.Type.SKILL_NOT_LEARNED);
        else
            EnableActiveSlotsAssigning();
    }

    void FetchSkill() {
        if (MPC.GetSkilllvlByIndex(SkillIndex) == 0)
            Skill = MPC.GetRawSkill(SkillIndex);
        else {
            Skill = MPC.GetActualSkill(SkillIndex);
        }
        if (Skill.lvl <= 0)//Not learned
            IconImage.color = MyColor.Grey;
        else {
            IconImage.color = MyColor.White;
            if (Skill.lvl == 5)
                transform.parent.GetComponent<Image>().color = MyColor.Purple;
        }
    }

    void LoadSkillIcon() {
        if (!Skill) {
            IconImage.sprite = null;
            Color ImageIconColor = IconImage.color;
            ImageIconColor.a = 0;
            IconImage.color = ImageIconColor;
        }
        else{
            IconImage.sprite = Resources.Load<Sprite>("SkillIcons/" + Skill.Name);
            Color ImageIconColor = IconImage.color;
            ImageIconColor.a = 255;
            IconImage.color = ImageIconColor;
        }
    }

    void AssignSkillPosition() {
        if (L_Tier1.Contains(SkillIndex))
            SP = SkillPosition.LT1;
        else if (L_Tier2.Contains(SkillIndex))
            SP = SkillPosition.LT2;
        else if (L_Tier3.Contains(SkillIndex))
            SP = SkillPosition.LT3;
        else if (R_Tier1.Contains(SkillIndex))
            SP = SkillPosition.RT1;
        else if (R_Tier2.Contains(SkillIndex))
            SP = SkillPosition.RT2;
        else if (R_Tier3.Contains(SkillIndex))
            SP = SkillPosition.RT3;
    }

    void SkillInfoUpdate() {
        if (Skill != null && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject) {
            transform.parent.GetComponent<Animator>().enabled = true;
            transform.parent.GetComponent<Animator>().SetBool("Blinking", true);
            if (SP == SkillPosition.LT1 || SP == SkillPosition.RT1)
                SI.Show(Skill, PathName, MeetRequirement(), Tier1_Req);
            else if (SP == SkillPosition.LT2 || SP == SkillPosition.RT2)
                SI.Show(Skill, PathName, MeetRequirement(), Tier2_Req);
            else if (SP == SkillPosition.LT3 || SP == SkillPosition.RT3)
                SI.Show(Skill, PathName, MeetRequirement(), Tier3_Req);

        } else {
            transform.parent.GetComponent<Animator>().SetBool("Blinking", false);
            transform.parent.GetComponent<Animator>().enabled = false;
            if (transform.parent.GetComponent<Image>().color.a != 0) {
                Color c = transform.parent.GetComponent<Image>().color;
                c.a = 0;
                transform.parent.GetComponent<Image>().color = c;
            }
        }
    }

    public bool MeetRequirement() {
        if (SP == SkillPosition.LT1 || SP == SkillPosition.RT1)
            return true;
        else if (SP == SkillPosition.LT2) {
            return GetTierSkillPoints(L_Tier1) >= Tier2_Req;
        } else if (SP == SkillPosition.RT2) {
            return GetTierSkillPoints(R_Tier1) >= Tier2_Req;
        } else if (SP == SkillPosition.LT3) {
            return GetTierSkillPoints(L_Tier2) + GetTierSkillPoints(L_Tier1) >= Tier3_Req;
        } else if (SP == SkillPosition.RT3) {
            return GetTierSkillPoints(R_Tier2) + GetTierSkillPoints(R_Tier1) >= Tier3_Req;
        }
        return false;
    }

    int GetTierSkillPoints(List<int> Tier) {
        int total = 0;
        foreach (int skillindex in Tier) {
            total += MPC.GetSkilllvlByIndex(skillindex);
        }
        return total;
    }

    void FetchPathName() {
        if (MPC.GetClass() == CLASS.Warrior) {
            if (SP == SkillPosition.LT1 || SP == SkillPosition.LT2 || SP == SkillPosition.LT3)
                PathName = WarriorPath.Berserker.ToString();
            else
                PathName = WarriorPath.Mountain.ToString();
        }

    }

    void EnableActiveSlotsAssigning() {
        foreach (ActiveSkillButtonController ASBC in ASBCs) {
            ASBC.EnableAssigning(GetComponent<SkillButton>());
        }
        SkillSubMenu.DisableSubmenuNavigation();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(ASBCs[0].gameObject);
    }

    public void DisableActiveSlotsAssigning() {
        foreach (ActiveSkillButtonController ASBC in ASBCs) {
            ASBC.DisableAssigning();
        }
    }

    void FetechActiveSlotSkills() {
        foreach (ActiveSkillButtonController ASBC in ASBCs) {
            ASBC.FetchSkill();
        }
    }

    public void RestoreCSCMPUIControl() {
        StartCoroutine(_RestoreCSCMPUIControl());
        //MPUI.AllowControl = true;
        //CSC.AllowControl = true;
    }

    IEnumerator _RestoreCSCMPUIControl() {
        yield return new WaitForSeconds(0.1f);
        MPUI.SyncActions = true;
        CSC.AllowControl = true;
    }
}
