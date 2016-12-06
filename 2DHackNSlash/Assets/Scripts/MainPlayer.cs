using UnityEngine;
using System.Collections;
using System;
using GreedyNameSpace;

public class MainPlayer : PlayerController {

    public GameObject PickedTarget = null;
    
    private MainPlayerUI MPUI;

    public GameObject[] TestObjects;

    Camera MainCamera;

    public Stats s;

    protected override void Awake() {
        base.Awake();
        foreach(GameObject to in TestObjects) {
            if(to!=null)
                to.GetComponent<EquipmentController>().InstantiateLoot(transform,new Vector2(1,1),0);
        }
        MPUI = transform.Find("MainPlayerUI").GetComponent<MainPlayerUI>();
        PlayerData = SaveLoadManager.LoadPlayerInfo(SaveLoadManager.SlotIndexToLoad);
        MainCamera = VisualHolder.GetComponentInChildren<Camera>();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        PickUpInUpdate();
    }

    protected override void Die() {
        base.Die();
        MainCamera.transform.parent = null;
        TopNotification.Push("Your soul is fading...",MyColor.Red);
        Destroy(transform.gameObject,4f);
        GameManager.LoadSceneWithWaitTime("Developing", 5f);
    }

    protected override void ControlUpdate() {
        if (Stunned || !Alive) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
            return;
        } else {
            if (WC!=null && GetCurrStats(StatsType.MANA) - WC.ManaCost < 0) {
                AttackVector = Vector2.zero;
                if (ControllerManager.AttackVector != Vector2.zero)
                    RedNotification.Push(RedNotification.Type.NO_MANA);
            } else {
                AttackVector = ControllerManager.AttackVector;
            }
            if (HasForce()) {
                //Debug.Log(rb.velocity.magnitude);
                MoveVector = Vector2.zero;
            } else {
                MoveVector = ControllerManager.MoveVector;
            }
            Direction = ControllerManager.Direction;
        }
    }

    void PickUpInUpdate() {
        if (PickedTarget != null && ControllerManager.AllowControlUpdate) {
            MPUI.transform.Find("PickUpNotify").gameObject.SetActive(true);
            if (Input.GetKeyDown(ControllerManager.Interact) || Input.GetKeyDown(ControllerManager.J_A)) {
                if (InventoryIsFull()) {
                    RedNotification.Push(RedNotification.Type.INVENTORY_FULL);
                } else {
                    int InventoryIndex = FirstAvailbleInventorySlot();
                    AddToInventory(InventoryIndex, PickedTarget.GetComponent<EquipmentController>().E);
                    Destroy(PickedTarget);
                    PickedTarget = null;
                }
            }
        } 
        else
            MPUI.transform.Find("PickUpNotify").gameObject.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("LootBox")) {
            PickedTarget = collider.transform.parent.gameObject;
            return;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("LootBox") && PickedTarget != null) {
            PickedTarget = null;
        }
    }




    //Stats Handling
    public int GetAvailableStatPoints() {
        return PlayerData.StatPoints;
    }

    public void AddStatPoint(string Stat) {
        //if (Stat == "Health")
        //    PlayerData.BaseHealth += StatModule.Health_Weight;
        //else if (Stat == "Mana")
        //    PlayerData.BaseMana += StatModule.Mana_Weight;
        //else if (Stat == "AD")
        //    PlayerData.BaseAD += StatModule.AD_Weight;
        //else if (Stat == "MD")
        //    PlayerData.BaseMD += StatModule.MD_Weight;
        //else if (Stat == "AttkSpd")
        //    PlayerData.BaseAttkSpd += StatModule.AttkSpd_Weight;
        //else if (Stat == "MoveSpd")
        //    PlayerData.BaseMoveSpd += StatModule.MoveSpd_Weight;
        //else if (Stat == "Defense")
        //    PlayerData.BaseDefense += StatModule.Defense_Weight;
        //else if (Stat == "CritChance")
        //    PlayerData.BaseCritChance += StatModule.CritChance_Weight;
        //else if (Stat == "CritDmgBounus")
        //    PlayerData.BaseCritDmgBounus += StatModule.CritDmgBounus_Weight;
        //else if (Stat == "LPH")
        //    PlayerData.BaseLPH += StatModule.LPH_Weight;
        //else if (Stat == "ManaRegen")
        //    PlayerData.BaseManaRegen += StatModule.ManaRegen_Weight;
        //PlayerData.StatPoints--;
        //SaveLoadManager.SaveCurrentPlayerInfo();
        //UpdateStats();
    }

    //Skills Handling
    public int GetAvailableSkillPoints() {
        return PlayerData.SkillPoints;
    }

    public void SetActiveSkillAt(int Slot, ActiveSkill active_skill) {
        if (active_skill == null)
            PlayerData.ActiveSlotData[Slot] = null;
        else
            PlayerData.ActiveSlotData[Slot] = active_skill.SD;
        SaveLoadManager.SaveCurrentPlayerInfo();
    }

    public ActiveSkill GetActiveSlotSkill(int Slot) {
        if (PlayerData.ActiveSlotData[Slot] == null || T_Actives.childCount == 0)
            return null;
        foreach (ActiveSkill AS in T_Actives.GetComponentsInChildren<ActiveSkill>())
            if (AS.SD.Name == PlayerData.ActiveSlotData[Slot].Name) {
                return AS;
            }
        return null;
    }

    public Skill GetSkillFromSkillTreeByIndex(int SkillIndex) {
        return SkillTree.transform.GetComponent<SkillTreeController>().SkillTree[SkillIndex];
    }

    public Skill SkillGetSkillFromPlayerByIndex(int SkillIndex) {
        Skill s_return = null;
        System.Type SkillType = SkillTree.transform.GetComponent<SkillTreeController>().SkillTree[SkillIndex].GetType();
        if (SkillTree.transform.GetComponent<SkillTreeController>().SkillTree[SkillIndex].GetComponent<Skill>().GetType().IsSubclassOf(typeof(ActiveSkill))) {
            foreach (ActiveSkill s in T_Actives.GetComponentsInChildren<ActiveSkill>())
                if (s.GetType() == SkillType) {
                    s_return = s;
                }
        } else {
            foreach (PassiveSkill s in T_Passives.GetComponentsInChildren<PassiveSkill>())
                if (s.GetType() == SkillType)
                    s_return = s;
        }
        return s_return;
    }

    public int GetSkilllvlByIndex(int SkillIndex) {
        return PlayerData.SkillTreelvls[SkillIndex];
    }

    public void LvlUpSkill(int SkillIndex) {
        PlayerData.SkillTreelvls[SkillIndex]++;
        PlayerData.SkillPoints--;
        SaveLoadManager.SaveCurrentPlayerInfo();
        UpdateSkillsState();
        UpdateStats();
    }

    //Equipment/Inventory Handling
    public bool InventoryIsFull() {
        return FirstAvailbleInventorySlot() == PlayerData.Inventory.Length;
    }

    public Equipment GetEquippedItem(EquipType Slot) {
        return PlayerData.Equipments[Slot];
    }
    public Equipment GetInventoryItem(int Slot) {
        return PlayerData.Inventory[Slot];
    }

    public bool Compatible(Equipment E) {
        if (E == null)
            return false;
        if (E.Class == Class.All)//Trinket
            return PlayerData.lvl >= E.LvlReq;
        return (PlayerData.lvl >= E.LvlReq && PlayerData.Class == E.Class);
    }

    public int FirstAvailbleInventorySlot() {
        for (int i = 0; i < PlayerData.Inventory.Length; i++) {
            if (PlayerData.Inventory[i] == null)
                return i;
        }
        return PlayerData.Inventory.Length;
    }

    public void Equip(Equipment E) {
        PlayerData.Equipments[E.EquipType] = E;
        GameObject equipPrefab = EquipmentController.ObtainPrefab(E, transform);
        EquipPrefabs[E.EquipType] = equipPrefab;
        UpdateStats();
        SaveLoadManager.SaveCurrentPlayerInfo();
    }

    public void UnEquip(EquipType Slot) {
        Destroy(EquipPrefabs[Slot]);
        PlayerData.Equipments[Slot] = null;
        UpdateStats();
        SaveLoadManager.SaveCurrentPlayerInfo();
    }

    public void AddToInventory(int Slot, Equipment E) {
        PlayerData.Inventory[Slot] = E;
        SaveLoadManager.SaveCurrentPlayerInfo();
    }

    public void RemoveFromInventory(int Slot, Equipment E) {
        PlayerData.Inventory[Slot] = null;
        SaveLoadManager.SaveCurrentPlayerInfo();
    }
}
