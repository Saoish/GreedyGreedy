using UnityEngine;
using System.Collections;
using GreedyNameSpace;
using System.Collections.Generic;
using System;
using Networking.Data;

[System.Serializable]
public class PlayerData {
    public int SlotIndex;
    public RGB SkinColor;
    public string Name;
    public CLASS Class;
    public int lvl;
    public int paragon_lvl;
    public int exp;
    public int souls;
    public int SkillPoints;
    public Stats BaseStats;
    public int[] SkillTreelvls;
    public string[] ActiveSlotData;
    public Equipment[] Equipments;

    public Equipment[] Inventory;

    public PlayerData() {
        SlotIndex = 0;
        SkinColor = new RGB();
        Name = "";
        Class = CLASS.Warrior;
        lvl = 1;
        paragon_lvl = 0;
        exp = 0;
        souls = 0;
        SkillPoints = 1;
        BaseStats = new Stats(Stats.InitStatsType.CHAR);
        SkillTreelvls = new int[Patch.SkillTreeSize];
        ActiveSlotData = new string[Patch.SkillSlots];
        for (int i = 0; i < ActiveSlotData.Length; i++) {
            ActiveSlotData[i] = "";
        }
        Equipments = new Equipment[Enum.GetValues(typeof(EQUIPTYPE)).Length];
        for (int i = 0; i < Equipments.Length; i++) {
            Equipments[i] = new Equipment();
        }
        Inventory = new Equipment[Patch.InventoryCapacity];
        for (int i = 0; i < Inventory.Length; i++) {
            Inventory[i] = new Equipment();
        }
    }
}
