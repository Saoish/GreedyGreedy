using UnityEngine;
using System.Collections;
using GreedyNameSpace;

[System.Serializable]
public class Equipment {
    public RARITY Rarity;//0 common and so on

    public string Name;
    public CLASS Class;//For non-trinket equipment only
    public EQUIPTYPE EquipType;

    public int Itemlvl = 1;
    public int LvlReq;
    public EQUIPSET Set;
    public int Reforged = 0; //NumofTime been reforged
    public Stats Stats;

    public string Description; //For run time fecthing

    public Equipment() {
        Name = "";
        Stats = new Stats(Stats.InitStatsType.EQUIP);
    }

    public bool isNull {
        get { return Name == ""; }
    }

}
