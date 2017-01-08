using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GreedyNameSpace;
using System.Linq;

public class EquipmentInfo : MonoBehaviour {
    public enum Mode { Inventory, Equipped };

    float Description_Interval = 191f;

    MainPlayer MPC;
    //Title
    Text Name;
    //Prefix
    Transform Icon;
    Text RarityType;
    Text Class;
    Text ItemLevel;
    //Stats
    Text Stats_Labels;
    Text Stats_Values;
    //Set
    Text SetName;
    Text SetList;
    Text SetInfo;
    //Description
    Text Description;
    //LevelRequired
    Text LevelRequired;

    GameObject CachedIconOJ;

    void OnEnable() {
        MPC = transform.parent.GetComponent<Tab_0>().MPC;

        Name = transform.Find("Title/Name").GetComponent<Text>();
        Icon = transform.Find("Prefix/Icon");
        RarityType = transform.Find("Prefix/GeneralInfo/RarityType").GetComponent<Text>();
        Class = transform.Find("Prefix/GeneralInfo/Class").GetComponent<Text>();
        ItemLevel = transform.Find("Prefix/GeneralInfo/ItemLevel").GetComponent<Text>();
        Stats_Labels = transform.Find("Stats/Labels").GetComponent<Text>();
        Stats_Values = transform.Find("Stats/Values").GetComponent<Text>();

        SetName = transform.Find("Set/SetName").GetComponent<Text>();
        SetList = transform.Find("Set/SetList").GetComponent<Text>();
        SetInfo = transform.Find("Set/SetInfo").GetComponent<Text>();

        Description = transform.Find("Description").GetComponent<Text>();

        LevelRequired = transform.Find("LevelRequired").GetComponent<Text>();
    }


    public void Show(Equipment E, Mode mode) {
        Reset();
        if (E.isNull) {            
            gameObject.SetActive(false);
            return;
        }

        UpdateNameRarityType(E);
        UpdateIcon(E);
        UpdateGeneralInfo(E);
        UpdateStats(E,mode);
        UpDateSet(E);

        UpdateDescription(E);
        UpdateLevelRequired(E);
        gameObject.SetActive(true);
    }

    public void Reset() {
        Name.text = "";
        Destroy(CachedIconOJ);
        RarityType.text = "";
        Class.text = "";
        ItemLevel.text = "";
        Stats_Labels.text = "";
        Stats_Values.text = "";

        SetName.text = "";
        SetList.text = "";
        SetInfo.text = "";

        Description.text = "";

        LevelRequired.text = "";
    }

    public void Disable() { gameObject.SetActive(false); }

    private void UpdateNameRarityType(Equipment E) {
        switch (E.Rarity) {
            case RARITY.Common:
                Name.color = MyColor.Common;
                RarityType.color = MyColor.Common;
                break;
            case RARITY.Fine:
                Name.color = MyColor.Fine;
                RarityType.color = MyColor.Fine;
                break;
            case RARITY.Pristine:
                Name.color = MyColor.Pristine;
                RarityType.color = MyColor.Pristine;
                break;
            case RARITY.Legendary:
                Name.color = MyColor.Legendary;
                RarityType.color = MyColor.Legendary;
                break;
            case RARITY.Mythic:
                Name.color = MyColor.Mythic;
                RarityType.color = MyColor.Mythic;
                break;
        }
        Name.text = E.Name;
        RarityType.text = E.Rarity.ToString() + " " + E.EquipType.ToString();
    }

    private void UpdateIcon(Equipment E) {
        CachedIconOJ = EquipmentController.ObtainEquippedIcon(E, Icon);        
    }

    private void UpdateGeneralInfo(Equipment E) {
        if (MPC.GetClass() == E.Class || E.Class == CLASS.All) {
            Class.color = MyColor.White;
        } else {
            Class.color = MyColor.Red;
        }
        Class.text = E.Class.ToString();
        ItemLevel.text = "Item Level: "+E.Itemlvl.ToString();
    }

    private void UpdateStats(Equipment E,Mode mode) {
        if(mode == Mode.Inventory) {            
            if (MPC.GetEquippedItem(E.EquipType) == null) {
                List<int> FieldsToShow = new List<int>();
                for (int s = 0; s < Stats.Size; s++) {
                    if (E.Stats.stats[s] > 0) {
                        FieldsToShow.Add(s);                        
                    }
                }                
                foreach(int s in FieldsToShow) {
                    StringPair sp = StatsType.GetStatsTypeString(s);
                    Stats_Labels.text += sp.F;
                    Stats_Values.text += MyText.Colofied("+ "+E.Stats.Get(s) + sp.S, "lime");
                    if (s != FieldsToShow.Last()) {                        
                        Stats_Labels.text +="\n";
                        Stats_Values.text +="\n";
                    } 
                }
            } else {
                Equipment To_Compare = MPC.GetEquippedItem(E.EquipType);
                List<int> FieldsToShow = new List<int>();
                for (int s = 0; s < Stats.Size; s++) {
                    if (E.Stats.stats[s] > 0)
                        FieldsToShow.Add(s);
                }
                for (int s = 0; s < Stats.Size; s++) {
                    if (!FieldsToShow.Contains(s) && To_Compare.Stats.stats[s] > 0)
                        FieldsToShow.Add(s);
                }
                foreach(int s in FieldsToShow) {
                    StringPair sp = StatsType.GetStatsTypeString(s);
                    Stats_Labels.text += sp.F;
                    float difference = E.Stats.Get(s) - To_Compare.Stats.Get(s);
                    if (difference>0) {
                        Stats_Values.text += MyText.Colofied("+" + E.Stats.Get(s) + sp.S +" (+"+difference.ToString("F1")+sp.S+")", "lime");
                    }else if (difference<0) {
                        Stats_Values.text += MyText.Colofied("+" + E.Stats.Get(s) + sp.S + " (" + difference.ToString("F1") + sp.S + ")", "red");
                    } else {
                        Stats_Values.text += MyText.Colofied("+" + E.Stats.Get(s) + sp.S + " (+" + difference.ToString("F1") + sp.S + ")", "white");
                    }
                    if (s != FieldsToShow.Last()) {
                        Stats_Labels.text += "\n";
                        Stats_Values.text += "\n";
                    }
                }
            }
        } else {
            List<int> FieldsToShow = new List<int>();
            for (int s = 0; s < Stats.Size; s++) {
                if (E.Stats.stats[s] > 0) {
                    FieldsToShow.Add(s);
                }
            }
            foreach (int s in FieldsToShow) {
                StringPair sp = StatsType.GetStatsTypeString(s);
                Stats_Labels.text += sp.F;
                Stats_Values.text += MyText.Colofied("+ " + E.Stats.Get(s) + sp.S, "white");
                if (s != FieldsToShow.Last()) {
                    Stats_Labels.text += "\n";
                    Stats_Values.text += "\n";
                }
            }
        }
    }

    void UpDateSet(Equipment E) {
        if (E.Set != EQUIPSET.None) {
            int count = 0;
            SetName.text = E.Set.ToString();
            Set s = ((GameObject)Resources.Load("SetPrefabs/"+E.Set.ToString())).GetComponent<Set>();
            List<string> EquippedNameList = new List<string>();
            for (int i = 0; i < System.Enum.GetValues(typeof(EQUIPTYPE)).Length; i++) {
                Equipment equipment = MPC.GetEquippedItem((EQUIPTYPE)i);
                if (equipment != null) {
                    EquippedNameList.Add(equipment.Name);
                }
            }
            foreach (var e in s.SetList) {
                if (EquippedNameList.Contains(e.E.Name)) {
                    SetList.text += MyText.Colofied(e.E.Name, "lime");
                    count++;
                } else {
                    SetList.text += MyText.Colofied(e.E.Name, "grey");
                }
                if (e != s.SetList.Last())
                    SetList.text += "\n";
            }
            foreach(Bounus b in s.Bounuses) {
                string b_info = "";
                b_info += "Set(" + b.condiction + "): ";
                switch (b.bounus_type) {
                    case Bounus.BounusType.Stats:
                        StringPair sp = StatsType.GetStatsTypeString(b.stats_bounus.stats_type);                        
                        b_info+= "Add "+ sp.F +" ";
                        switch (b.stats_bounus.value_type) {                            
                            case SetStatsField.ValueType.Raw:
                                b_info += b.stats_bounus.value;
                                break;
                            case SetStatsField.ValueType.Percentage:
                                b_info += b.stats_bounus.value + "%";
                                break;
                        }                           
                        break;
                    case Bounus.BounusType.Passive:
                        b.passive_bounus.GenerateDescription();
                        b_info += b.passive_bounus.Name+" ("+b.passive_bounus.Description+")";
                        break;
                }
                if (count >= b.condiction)
                    SetInfo.text += MyText.Colofied(b_info, "lime");
                else
                    SetInfo.text += MyText.Colofied(b_info, "grey");
                if (b != s.Bounuses.Last())
                    SetInfo.text += "\n";
            }            
            transform.Find("Set").gameObject.SetActive(true);
        } else {
            transform.Find("Set").gameObject.SetActive(false);
        }
            
    }

    void UpdateDescription(Equipment E) {
        Description.text = E.Description;
    }

    void UpdateLevelRequired(Equipment E) {
        if (MPC.Getlvl() >= E.LvlReq)
            LevelRequired.color = MyColor.White;
        else
            LevelRequired.color = MyColor.Red;
        LevelRequired.text = "Level Required: " + E.LvlReq;
    }

}
