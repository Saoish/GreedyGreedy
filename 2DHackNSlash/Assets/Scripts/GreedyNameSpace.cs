using UnityEngine;
using System.Collections;


namespace GreedyNameSpace {
    public static class Patch {
        public static float Version = 1.46f;

        public static int LvlCap = 50;
        public static float Itemlvl_Scale = 0.1f;
        public static int InventoryCapacity = 40;
        public static int SkillTreeSize = 18;
        public static int SkillSlots = 4;
        public static int CharacterSlots = 6;
        public static int MaxItemlvl = LvlCap + (int)Rarity.Legendary;
    }
    public enum Class{
        Warrior,
        Mage,
        Rogue,
        All
    };
    public enum EquipType {
        Helmet,
        Chest,
        Shackle,
        Weapon,
        Trinket
    };
    public enum WeaponType {
        Axe,
        GreatSword,
        SwordShield,
        Orb,
        Staff,
        Dagger,
        Katana
    }
    public enum Rarity {
        Common = 0,
        Fine = 2,
        Perfect = 4,
        Mythic = 6,
        Legendary = 8
    }
    public enum RarityRate {
        Common = 100,
        Fine = 20,
        Perfect = 10,
        Mythic = 5,
        Legendary = 1
    }
    public enum StatsType {
        HEALTH,
        MANA,
        AD,
        MD,
        DEFENSE,
        ATTACK_SPEED,
        MOVE_SPEED,
        CRIT_CHANCE,
        CRIT_DMG,
        LPH,
        CDR,
        HEALTH_REGEN,
        MANA_REGEN
    }
    public enum InitStatsType {
        CHAR,
        EQUIP
    }
    public enum EquipSet {
        None
    }
}
