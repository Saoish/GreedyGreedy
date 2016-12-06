using UnityEngine;
using System.Collections;
using GreedyNameSpace;

[System.Serializable]
public class Equipment{
    public Rarity Rarity;//0 common and so on

    public string Name;
    public Class Class;//For non-trinket equipment only
    public EquipType EquipType;

    public int Itemlvl;
    public int LvlReq;
    public EquipSet Set; //NumofTime been rerolled
    public int Reforged = 0; //NumofTime been reforged
    public Stats Stats;

    public string Description; //For run time fecthing

    public Equipment() {
        Stats = new Stats(InitStatsType.EQUIP);
    }

}
