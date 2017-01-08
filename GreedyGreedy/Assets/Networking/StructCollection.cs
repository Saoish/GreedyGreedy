using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;

[System.Serializable]
public struct BoxTrigger {
    public float Start;
    public float End;
}

[System.Serializable]
public struct StatsRangeField {
    public STATSTYPE type;
    public Vector2 stats_range;
}
[System.Serializable]
public struct NChooseNFields {
    public int Choose;
    public List<StatsRangeField> Fields;
}

[System.Serializable]
public struct SetStatsField {
    public enum ValueType {
        Raw,
        Percentage
    }
    public STATSTYPE stats_type;
    public ValueType value_type;
    public float value;
}

[System.Serializable]
public struct Loot {
    public float Rate;
    public GameObject Item;
}

[System.Serializable]
public struct StringPair {
    public string F;
    public string S;
    public StringPair(string F, string S) {
        this.F = F;
        this.S = S;
    }
}


//[System.Serializable]
//public struct PlayerData {
//    public int SlotIndex;
//    public string Name;
//    public CLASS Class;
//    public int lvl;
//    public int paragon_lvl;
//    public int exp;

//    public int souls;

//    public int SkillPoints;

//    public Stats BaseStats;

//    public int[] SkillTreelvls;

//    public string[] ActiveSlotData;

//    public Dictionary<EQUIPTYPE, Equipment> Equipments;

//    public Equipment[] Inventory;
//}