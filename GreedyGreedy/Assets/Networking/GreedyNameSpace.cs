using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GreedyNameSpace {
    public static class Patch {
        public static float Version = 1.46f;

        public static int LvlCap = 50;
        public static float Itemlvl_Scale = 0.1f;
        public static int InventoryCapacity = 40;
        public static int SkillTreeSize = 18;
        public static int SkillSlots = 4;
        public static int CharacterSlots = 6;
        public static int MaxItemlvl = LvlCap + (int)RARITY.Mythic;
        public static int MaxSkilllvl = 5;

        public static int Tier1_Req = 0;
        public static int Tier2_Req = 10;
        public static int Tier3_Req = 25;
    }
    public enum CLASS {
        Warrior,
        Mage,
        Rogue,
        All
    };
    public enum WarriorPath {
        Berserker,
        Mountain
    }
    public enum STATSTYPE {
        HEALTH,
        ESSENSE,
        DAMAGE,
        DEFENSE,
        PENETRATION,
        ATTACK_SPEED,
        MOVE_SPEED,
        CRIT_CHANCE,
        CRIT_DMG,
        LPH,
        HASTE,
        HEALTH_REGEN,
        ESSENSE_REGEN
    }
    public static class StatsType {
        public static StringPair GetStatsTypeString(STATSTYPE type) {
            return GetStatsTypeString((int)type);
        }
        public static StringPair GetStatsTypeString(int type) {
            STATSTYPE _type = (STATSTYPE)type;
            switch (_type) {
                case STATSTYPE.HEALTH:
                    return new StringPair("Health", "");
                case STATSTYPE.ESSENSE:
                    return new StringPair("Essense", "");
                case STATSTYPE.DAMAGE:
                    return new StringPair("Damage", "");
                case STATSTYPE.ATTACK_SPEED:
                    return new StringPair("Attack Speed", "%");
                case STATSTYPE.MOVE_SPEED:
                    return new StringPair("Move Speed", "%");
                case STATSTYPE.DEFENSE:
                    return new StringPair("Defense", "%");
                case STATSTYPE.PENETRATION:
                    return new StringPair("Penetration", "%");
                case STATSTYPE.CRIT_CHANCE:
                    return new StringPair("Critical Chance", "%");
                case STATSTYPE.CRIT_DMG:
                    return new StringPair("Critical Damaga", "%");
                case STATSTYPE.LPH:
                    return new StringPair("Life/Hit", "%");
                case STATSTYPE.HASTE:
                    return new StringPair("Haste", "%");
                case STATSTYPE.HEALTH_REGEN:
                    return new StringPair("Health Regen", "/s");
                case STATSTYPE.ESSENSE_REGEN:
                    return new StringPair("Essense Regen", "/s");
            }
            return new StringPair("", "");
        }
    }
    public enum EQUIPTYPE {
        Helmet,
        Chest,
        Shackle,
        Weapon,
        Trinket
    };
    public enum WEAPONTYPE {
        Axe,
        GreatSword,
        SwordShield,
        Orb,
        Staff,
        Dagger,
        Katana
    }
    public enum RARITY {
        Common = 0,
        Fine = 2,
        Pristine = 4,
        Legendary = 6,
        Mythic = 8
    }
    public enum RARITYRATE {
        Common = 100,
        Fine = 20,
        Pristine = 10,
        Legendary = 5,
        Mythic = 1
    }
    public enum EQUIPSET {
        None,
        Skeleton
    }   


    //--------------
    public static class DamageCalculation {
        public static float GetNotTrueDamage(float RawDamage, float TargetDefense, float SelfPenetration) {
            float Diff = TargetDefense - SelfPenetration;
            if (Diff < 0) {
                Diff = 0;
            }
            float reduced_dmg = RawDamage * Diff / 100;
            return RawDamage - reduced_dmg >= 1 ? Mathf.Ceil(RawDamage - reduced_dmg) : 1;
        }
    }

    public static class MyText {
        static public string Colofied<T>(T content, string color) {
            return "<color=" + color + ">" + content.ToString() + "</color>";
        }

    }

    public static class MyColor {
        static public Color White = Color.white;
        static public Color Cyan = Color.cyan;
        static public Color Yellow = Color.yellow;
        static public Color Orange = new Color(1f, 0.65f, 0f, 1f);
        static public Color Green = Color.green;
        static public Color Red = Color.red;
        static public Color Blue = Color.blue;
        static public Color Grey = Color.grey;
        static public Color Purple = new Color(1f, 0, 1f, 1f);
        static public Color Pink = new Color(1f, 0.4f, 0.7f, 1f);

        static public Color Common = Color.white;
        static public Color Fine = Color.cyan;
        static public Color Pristine = Color.yellow;
        static public Color Legendary = MyColor.Orange;
        static public Color Mythic = MyColor.Purple;
    }

    public enum ObjectIdentity { Monster, Main, Friend, Enemy };
}
