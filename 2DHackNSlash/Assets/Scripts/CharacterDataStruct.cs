using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;

[System.Serializable]
public struct CharacterDataStruct{
    public int SlotIndex;
    public string Name;
    public Class Class;
    public int lvl;
    public int paragon_lvl;
    public int exp;

    public int souls;

    public int StatPoints;
    public int SkillPoints;

    public Stats BaseStats;

    public int[] SkillTreelvls;

    public SkillData[] ActiveSlotData;

    public Dictionary<EquipType, Equipment> Equipments;

    public Equipment[] Inventory;
}
