using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GreedyNameSpace;

public class EquipmentInfo : MonoBehaviour {
    public enum Mode { Inventory, Equipped };

    float Description_Interval = 191f;

    MainPlayer MPC;
    Text Name;
    Text Class;
    Text EquipType;
    Text Itemlvl;
    Text LvlReq;

    Text[] StatsFields;

    Text Description;

    void OnEnable() {
        MPC = transform.parent.GetComponent<Tab_0>().MPC;

        Name = transform.Find("Name").GetComponent<Text>();
        Class = transform.Find("Class").GetComponent<Text>();
        EquipType = transform.Find("EquipType").GetComponent<Text>();
        Itemlvl = transform.Find("Itemlvl").GetComponent<Text>();
        LvlReq = transform.Find("LvlReq").GetComponent<Text>();

        StatsFields = transform.Find("Stats").GetComponentsInChildren<Text>();

        Description = transform.Find("Description").GetComponent<Text>();
    }


    public void Show(Equipment E, Mode mode) {
        if (E == null) {
            gameObject.SetActive(false);
            return;
        }

        SetName(E);
        SetClass(E);
        SetEquipType(E);
        SetItemLvl(E);
        SetLvlReq(E);
        SetStatsAndDescription(E, mode);
        gameObject.SetActive(true);
    }

    public void Reset() {
        Name.text = "";
        Class.text = "";
        EquipType.text = "";
        Itemlvl.text = "";
        LvlReq.text = "";
        foreach (Text t in StatsFields)
            t.text = "";
    }

    public void Disable() { gameObject.SetActive(false); }


    private void SetName(Equipment E) {
        switch (E.Rarity) {
            case Rarity.Common:
                Name.color = MyColor.Common;
                break;
            case Rarity.Fine:
                Name.color = MyColor.Fine;
                break;
            case Rarity.Pristine:
                Name.color = MyColor.Pristine;
                break;
            case Rarity.Legendary:
                Name.color = MyColor.Legendary;
                break;
            case Rarity.Mythic:
                Name.color = MyColor.Mythic;
                break;
        }
        Name.text = E.Name;
    }

    private void SetClass(Equipment E) {
        if (MPC.GetClass() == E.Class || E.Class == GreedyNameSpace.Class.All) {
            Class.color = MyColor.Green;
        } else {
            Class.color = MyColor.Red;
        }
        Class.text = E.Class.ToString();
    }

    private void SetEquipType(Equipment E) {
        EquipType.color = MyColor.White;
        EquipType.text = E.EquipType.ToString();
    }

    private void SetItemLvl(Equipment E) {
        switch (E.Rarity) {
            case Rarity.Common:
                Itemlvl.color = MyColor.Common;
                break;
            case Rarity.Fine:
                Itemlvl.color = MyColor.Fine;
                break;
            case Rarity.Pristine:
                Itemlvl.color = MyColor.Pristine;
                break;
            case Rarity.Legendary:
                Itemlvl.color = MyColor.Legendary;
                break;
            case Rarity.Mythic:
                Itemlvl.color = MyColor.Mythic;
                break;
        }
        Itemlvl.text = "Item level " + E.Itemlvl.ToString();
    }

    private void SetLvlReq(Equipment E) {
        if (MPC.Getlvl() >= E.LvlReq) {
            LvlReq.color = MyColor.Green;
        } else
            LvlReq.color = MyColor.Red;
        LvlReq.text = "Require lvl " + E.LvlReq.ToString();
    }

    private void SetStatsAndDescription(Equipment E,Mode mode) {
        int field = 0;
        if (mode == Mode.Inventory) {
            if (MPC.GetEquippedItem(E.EquipType) == null) {//No equipment on this type
                for (int s = 0; s < Stats.Size; s++) {
                    if (E.Stats.stats[s] > 0) {
                        StatsFields[field].color = MyColor.Green;
                        StringPair sp = GetStatsTypeString(s);
                        StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S;
                        field++;
                    }                
                }
            } else {
                Equipment To_Compare = MPC.GetEquippedItem(E.EquipType);
                List<int> Fields = new List<int>();
                for (int s = 0; s<Stats.Size; s++) {
                    if (E.Stats.stats[s] > 0)
                        Fields.Add(s);                    
                }
                for(int s = 0; s < Stats.Size; s++) {
                    if (!Fields.Contains(s) && To_Compare.Stats.stats[s] > 0)
                        Fields.Add(s);
                }
                foreach(int s in Fields) {
                    StringPair sp = GetStatsTypeString(s);
                    if (E.Stats.Get(s) > To_Compare.Stats.Get(s)) {
                        StatsFields[field].color = MyColor.Green;
                        StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S + " (+" + (sp.S == "" ? (E.Stats.Get(s) - To_Compare.Stats.Get(s)).ToString() : (E.Stats.Get(s) - To_Compare.Stats.Get(s)).ToString("F1")) + sp.S + ")";
                        //StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S + " (+" + (E.Stats.Get(s) - To_Compare.Stats.Get(s)) + sp.S + ")";
                    } else if (E.Stats.Get(s) < To_Compare.Stats.Get(s)) {
                        StatsFields[field].color = MyColor.Red;
                        StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S + " (" + (sp.S == "" ? (E.Stats.Get(s) - To_Compare.Stats.Get(s)).ToString() : (E.Stats.Get(s) - To_Compare.Stats.Get(s)).ToString("F1")) + sp.S + ")";
                        //StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S + " (" + (E.Stats.Get(s) - To_Compare.Stats.Get(s)) + sp.S + ")";
                    } else {
                        StatsFields[field].color = MyColor.White;
                        StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S;
                    }
                    field++;
                }
            }
        } else {
            for (int s = 0; s < Stats.Size; s++) {
                if (E.Stats.stats[s] > 0) {
                    StatsFields[field].color = MyColor.Green;
                    StringPair sp = GetStatsTypeString(s);
                    StatsFields[field].text = sp.F + " +" + E.Stats.Get(s) + sp.S;
                    field++;
                }
            }
        }
        SetDescription(E, mode, field);
    }

    void SetDescription(Equipment E, Mode mode,int LastField) {
        if (E.Set != EquipSet.None) {
            Description.color = MyColor.Purple;
        } else
            Description.color = MyColor.White;
        Description.transform.localPosition = StatsFields[LastField].transform.localPosition - new Vector3(0,Description_Interval,0);
        Description.text = E.Description;
    }


    StringPair GetStatsTypeString(int type) {
        StatsType _type = (StatsType)type;
        switch (_type) {
            case StatsType.HEALTH:
                return new StringPair("Health","");
            case StatsType.MANA:
                return new StringPair("Mana","");
            case StatsType.AD:
                return new StringPair("AD","");
            case StatsType.MD:
                return new StringPair("MD","");
            case StatsType.ATTACK_SPEED:
                return new StringPair("Attack Speed","%");
            case StatsType.MOVE_SPEED:
                return new StringPair("Move Speed","%");
            case StatsType.DEFENSE:
                return new StringPair("Defense", "%");
            case StatsType.CRIT_CHANCE:
                return new StringPair("Crit Chance", "%");
            case StatsType.CRIT_DMG:
                return new StringPair("Crit Damaga", "%");
            case StatsType.LPH:
                return new StringPair("Life/Hit", "%");
            case StatsType.CDR:
                return new StringPair("Cooldown Reduction", "%");
            case StatsType.HEALTH_REGEN:
                return new StringPair("Health Regen", "/s");
            case StatsType.MANA_REGEN:
                return new StringPair("Mana Regen", "/s");
        }   
        return new StringPair("","");
    } 







}
