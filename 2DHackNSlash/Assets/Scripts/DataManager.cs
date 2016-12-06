using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using GreedyNameSpace;


public static class DataManager {
    static private string FilePath = "Save/username_save.txt";
    static private string DataSeperator = ",";
    static private string SlotSeperator = "------SlotSeperator------";
    static private string CategorySeperator = "---CategorySeperator---";
    //static private string SkillPathSeperator = "-ActiveSkillSep-";
    static private string EquipmentSeperator = "-EquipSeperator-";

    static private int NumOfSlot = Patch.CharacterSlots;

    static private int InventoryCapacity = Patch.InventoryCapacity;

    static private int SkillTreeSize = Patch.SkillTreeSize;

    static private int ActiveSkillSlots = Patch.SkillSlots;

    static private CharacterDataStruct[] CharacterData = new CharacterDataStruct[NumOfSlot];

    public static void Save() {
        StreamWriter SS = new StreamWriter(FilePath);
        for (int i = 0; i < NumOfSlot; i++) {
            WriteCharacter(SS, i);
            WriteBaseStats(SS, i);
            WriteSkillTreelvls(SS, i);
            WriteActiveSkills(SS, i);
            WriteEquipments(SS, i);
            WriteInventory(SS, i);
            if (i != NumOfSlot - 1)
                SS.Write(Environment.NewLine + SlotSeperator + Environment.NewLine);
        }

        SS.Close();
    }

    public static void Load() {
        if (!File.Exists(FilePath)) {//First Time Login
            InitSave();
        }
        StreamReader LoadStream = new StreamReader(FilePath);
        string SlotData = "";

        while (!LoadStream.EndOfStream) {
            SlotData += LoadStream.ReadLine();
        }
        string[] CharactersData = Regex.Split(SlotData, SlotSeperator);
        for (int i = 0; i < NumOfSlot; i++) {
            string[] ThisCharacterData = Regex.Split(CharactersData[i], CategorySeperator);
            int n = 0;
            string HeaderData = ThisCharacterData[n++];
            string BaseData = ThisCharacterData[n++];
            string SkillTreelvlsData = ThisCharacterData[n++];
            string ActiveSkillsData = ThisCharacterData[n++];
            string EquipData = ThisCharacterData[n++];
            string InventoryData = ThisCharacterData[n++];
            ReadCharacter(i, HeaderData);
            ReadBaseStats(i, BaseData);
            ReadSkillTreelvls(i, SkillTreelvlsData);
            ReadActiveSkills(i, ActiveSkillsData);
            ReadEquipments(i, EquipData);
            ReadInventory(i, InventoryData);
        }
        LoadStream.Close();
    }


    public static void SaveCharacter(CharacterDataStruct PlayerData) {
        CharacterData[PlayerData.SlotIndex] = PlayerData;
    }

    public static CharacterDataStruct LoadCharacter(int SlotIndex) {
        return CharacterData[SlotIndex];
    }



    //------------------Writing
    private static void WriteCharacter(StreamWriter SW, int SlotIndex) {
        SW.Write(
            CharacterData[SlotIndex].SlotIndex + DataSeperator +
            CharacterData[SlotIndex].Name + DataSeperator +
            (int)CharacterData[SlotIndex].Class + DataSeperator +
            CharacterData[SlotIndex].lvl + DataSeperator +
            CharacterData[SlotIndex].paragon_lvl + DataSeperator +
            CharacterData[SlotIndex].exp + DataSeperator +

            CharacterData[SlotIndex].souls + DataSeperator +

            CharacterData[SlotIndex].StatPoints + DataSeperator +
            CharacterData[SlotIndex].SkillPoints +

            Environment.NewLine + CategorySeperator + Environment.NewLine);
    }

    private static void WriteBaseStats(StreamWriter SW, int SlotIndex) {
        for(int b = 0; b<Stats.Size; b++) {
            if (b == Stats.Size - 1)
                SW.Write(CharacterData[SlotIndex].BaseStats.Get(b));
            else
                SW.Write(CharacterData[SlotIndex].BaseStats.Get(b) + DataSeperator);
        }
        SW.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);
    }

    private static void WriteSkillTreelvls(StreamWriter SW, int SlotIndex) {
        for(int s = 0; s < SkillTreeSize; s++) {
            if (s == SkillTreeSize - 1)
                SW.Write(CharacterData[SlotIndex].SkillTreelvls[s]);
            else
                SW.Write(CharacterData[SlotIndex].SkillTreelvls[s] + DataSeperator);
        }
        SW.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);
    }

    private static void WriteActiveSkills(StreamWriter SW, int SlotIndex) {
        for(int s = 0; s < ActiveSkillSlots; s++) {
            if (CharacterData[SlotIndex].ActiveSlotData[s]!=null){
                if (s == ActiveSkillSlots - 1)
                    SW.Write(CharacterData[SlotIndex].ActiveSlotData[s].Name);
                else
                    SW.Write(CharacterData[SlotIndex].ActiveSlotData[s].Name + DataSeperator);
            } else {
                if (s == ActiveSkillSlots - 1)
                    SW.Write("null");
                else
                    SW.Write("null" + DataSeperator);
            }
        }
        SW.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);
    }

    private static void WriteEquipments(StreamWriter SW, int SlotIndex) {
        foreach (var e in CharacterData[SlotIndex].Equipments) {
            if (e.Value!=null) {
                SW.Write(
                    (int)CharacterData[SlotIndex].Equipments[e.Key].Rarity + DataSeperator +
                    CharacterData[SlotIndex].Equipments[e.Key].Name + DataSeperator +
                    (int)CharacterData[SlotIndex].Equipments[e.Key].Class + DataSeperator +
                    (int)CharacterData[SlotIndex].Equipments[e.Key].EquipType + DataSeperator +
                    CharacterData[SlotIndex].Equipments[e.Key].Itemlvl + DataSeperator +
                    CharacterData[SlotIndex].Equipments[e.Key].LvlReq + DataSeperator +
                    (int)CharacterData[SlotIndex].Equipments[e.Key].Set + DataSeperator +
                    CharacterData[SlotIndex].Equipments[e.Key].Reforged + DataSeperator);

                for (int i = 0;i<Stats.Size;i++) {
                    if(i<Stats.Size-1)
                        SW.Write(CharacterData[SlotIndex].Equipments[e.Key].Stats.Get(i) + DataSeperator);
                    else
                        SW.Write(CharacterData[SlotIndex].Equipments[e.Key].Stats.Get(i));
                }
            } else {
                SW.Write("null");
            }
            if (e.Key !=EquipType.Trinket)
                SW.Write(Environment.NewLine + EquipmentSeperator + Environment.NewLine);
        }
        SW.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);
    }

    private static void WriteInventory(StreamWriter SW, int SlotIndex) {
        for(int i =0;i< CharacterData[SlotIndex].Inventory.Length;i++) {
            if (CharacterData[SlotIndex].Inventory[i] != null) {
                SW.Write(
                    (int)CharacterData[SlotIndex].Inventory[i].Rarity + DataSeperator +
                    CharacterData[SlotIndex].Inventory[i].Name + DataSeperator +
                    (int)CharacterData[SlotIndex].Inventory[i].Class + DataSeperator +
                    (int)CharacterData[SlotIndex].Inventory[i].EquipType + DataSeperator +
                    CharacterData[SlotIndex].Inventory[i].Itemlvl + DataSeperator +
                    CharacterData[SlotIndex].Inventory[i].LvlReq + DataSeperator +
                    (int)CharacterData[SlotIndex].Inventory[i].Set + DataSeperator +
                    CharacterData[SlotIndex].Inventory[i].Reforged + DataSeperator);

                for (int j = 0; j < Stats.Size; j++) {
                    if (j < Stats.Size - 1)
                        SW.Write(CharacterData[SlotIndex].Inventory[i].Stats.Get(j) + DataSeperator);
                    else
                        SW.Write(CharacterData[SlotIndex].Inventory[i].Stats.Get(j));
                }
            } else {
                SW.Write("null");
            }
            if(i < CharacterData[SlotIndex].Inventory.Length - 1) {
                SW.Write(Environment.NewLine + EquipmentSeperator + Environment.NewLine);
            }
        }
        //SW.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);
    }



    //------------------Reading
    private static void ReadCharacter(int slotIndex, string BaseData) {
        string[] S_BaseData = Regex.Split(BaseData, DataSeperator);
        int i = 0;
        CharacterData[slotIndex].SlotIndex = int.Parse(S_BaseData[i++]);
        CharacterData[slotIndex].Name = S_BaseData[i++];
        CharacterData[slotIndex].Class = (Class)int.Parse(S_BaseData[i++]);
        CharacterData[slotIndex].lvl = int.Parse(S_BaseData[i++]);
        CharacterData[slotIndex].paragon_lvl = int.Parse(S_BaseData[i++]);
        CharacterData[slotIndex].exp = int.Parse(S_BaseData[i++]);

        CharacterData[slotIndex].souls = int.Parse(S_BaseData[i++]);

        CharacterData[slotIndex].StatPoints = int.Parse(S_BaseData[i++]);
        CharacterData[slotIndex].SkillPoints = int.Parse(S_BaseData[i++]);
    }

    private static void ReadBaseStats(int SlotIndex, string BaseStatsData) {
        string[] S_BaseStatsData = Regex.Split(BaseStatsData, DataSeperator);
        CharacterData[SlotIndex].BaseStats = new Stats();
        for(int b = 0; b < Stats.Size; b++) {
            CharacterData[SlotIndex].BaseStats.Set(b, float.Parse(S_BaseStatsData[b]));
        }
    }

    private static void ReadSkillTreelvls(int SlotIndex, string SkillTreelvlsData) {
        string[] S_SkillTreelvlsData = Regex.Split(SkillTreelvlsData, DataSeperator);
        CharacterData[SlotIndex].SkillTreelvls = new int[SkillTreeSize];
        for (int s = 0; s < SkillTreeSize; s++) {
            CharacterData[SlotIndex].SkillTreelvls[s] = int.Parse(S_SkillTreelvlsData[s]);
        }
    }

    private static void ReadActiveSkills(int SlotIndex, string ActiveSkillsData) {
        string[] S_ActiveSkillsData = Regex.Split(ActiveSkillsData, DataSeperator);
        CharacterData[SlotIndex].ActiveSlotData = new SkillData[ActiveSkillSlots];
        for(int s = 0; s < ActiveSkillSlots; s++) {
            if (S_ActiveSkillsData[s] == "null")
                CharacterData[SlotIndex].ActiveSlotData[s] = null;
            else {
                SkillData SD = new SkillData(S_ActiveSkillsData[s]);
                CharacterData[SlotIndex].ActiveSlotData[s] = SD;
            }
        }
    }

    private static void ReadEquipments(int slotIndex, string EquipData) {
        string[] S_EquipData = Regex.Split(EquipData, EquipmentSeperator);
        CharacterData[slotIndex].Equipments = new Dictionary<EquipType, Equipment>();
        for (int i = 0; i < S_EquipData.Length; i++) {
            switch (i) {
                case 0:
                    CharacterData[slotIndex].Equipments[EquipType.Helmet] = ReadOnePieceOfEquip(S_EquipData[i]);
                    break;
                case 1:
                    CharacterData[slotIndex].Equipments[EquipType.Chest] = ReadOnePieceOfEquip(S_EquipData[i]);
                    break;
                case 2:
                    CharacterData[slotIndex].Equipments[EquipType.Shackle] = ReadOnePieceOfEquip(S_EquipData[i]);
                    break;
                case 3:
                    CharacterData[slotIndex].Equipments[EquipType.Weapon] = ReadOnePieceOfEquip(S_EquipData[i]);
                    break;
                case 4:
                    CharacterData[slotIndex].Equipments[EquipType.Trinket] = ReadOnePieceOfEquip(S_EquipData[i]);
                    break;
            }
        }
    }

    static private void ReadInventory(int SlotIndex, string InventoryData) {
        string[] S_EquipData = Regex.Split(InventoryData, EquipmentSeperator);
        CharacterData[SlotIndex].Inventory = new Equipment[InventoryCapacity];
        for(int i =0;i< CharacterData[SlotIndex].Inventory.Length; i++) {
            CharacterData[SlotIndex].Inventory[i] = ReadOnePieceOfEquip(S_EquipData[i]);
        }
    }

    private static Equipment ReadOnePieceOfEquip(string PieceEquipData) {
        if (PieceEquipData == "null")
            return null;
        Equipment E = new Equipment();
        string[] S_PieceEquipData = Regex.Split(PieceEquipData, DataSeperator);
        int i = 0;
        E.Rarity = (Rarity)int.Parse(S_PieceEquipData[i++]);
        E.Name = S_PieceEquipData[i++];
        E.Class = (Class)int.Parse(S_PieceEquipData[i++]);
        E.EquipType = (EquipType)int.Parse(S_PieceEquipData[i++]);
        E.Itemlvl = int.Parse(S_PieceEquipData[i++]);
        E.LvlReq = int.Parse(S_PieceEquipData[i++]);
        E.Set = (EquipSet)int.Parse(S_PieceEquipData[i++]);
        E.Reforged = int.Parse(S_PieceEquipData[i++]);

        for(int j = 0; j < Stats.Size; j++) {
            E.Stats.Set(j, float.Parse(S_PieceEquipData[i++]));
        }
        return E;
    }

    //----------Save Init
    private static void InitSave() {//For server to init save file format
        StreamWriter SaveStream = new StreamWriter(FilePath);
        for (int slotIndex = 0; slotIndex < NumOfSlot; slotIndex++) {
            SaveStream.Write(
            //Character
            slotIndex + DataSeperator +
            "null" + DataSeperator +
            -1 + DataSeperator +
            CharacterData[slotIndex].lvl + DataSeperator +
            CharacterData[slotIndex].paragon_lvl + DataSeperator +
            CharacterData[slotIndex].exp + DataSeperator +
            CharacterData[slotIndex].souls + DataSeperator +
            CharacterData[slotIndex].StatPoints + DataSeperator +
            CharacterData[slotIndex].SkillPoints);

            SaveStream.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);

            //Base Stats
            Stats DefaultBase = new Stats();
            for (int b = 0; b < Stats.Size; b++) {
                if (b < Stats.Size - 1)
                    SaveStream.Write(DefaultBase.Get(b) + DataSeperator);
                else
                    SaveStream.Write(DefaultBase.Get(b));
            }

            SaveStream.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);

            //Skill Tree
            for (int s = 0; s < SkillTreeSize; s++) {
                if (s == SkillTreeSize - 1)
                    SaveStream.Write(0);
                else
                    SaveStream.Write(0 + DataSeperator);
            }
            SaveStream.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);

            //Active Skills
            for(int s = 0; s < ActiveSkillSlots; s++) {
                if (s == ActiveSkillSlots - 1)
                    SaveStream.Write("null");
                else
                    SaveStream.Write("null" + DataSeperator);
            }
            SaveStream.Write(Environment.NewLine + CategorySeperator + Environment.NewLine);

            //Equiped Items
            SaveStream.Write(
                "null" +
                Environment.NewLine + EquipmentSeperator + Environment.NewLine +
                "null" +
                Environment.NewLine + EquipmentSeperator + Environment.NewLine +
                "null" +
                Environment.NewLine + EquipmentSeperator + Environment.NewLine +
                "null" +
                Environment.NewLine + EquipmentSeperator + Environment.NewLine +
                "null" +

                Environment.NewLine + CategorySeperator + Environment.NewLine
            );

            //Inventory
            for(int i = 0; i < InventoryCapacity; i++) {
                SaveStream.Write("null");
                if (i < InventoryCapacity-1)
                    SaveStream.Write(Environment.NewLine+EquipmentSeperator + Environment.NewLine);
            }

            if (slotIndex != NumOfSlot - 1) {
                SaveStream.Write(Environment.NewLine+SlotSeperator + Environment.NewLine);
            }

        }
        SaveStream.Close();
    }
}
