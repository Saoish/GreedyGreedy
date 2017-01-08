using UnityEngine;
using System.Collections;

public static class StatModule {
    public static float Default_Base_Health = 100;
    public static float Default_Base_Mana = 100;
    public static float Default_Base_AD = 0;
    public static float Default_Base_MD = 0;
    public static float Default_Base_AttkSpd = 100;
    public static float Default_Base_MoveSpd = 100;
    public static float Default_Base_Defense = 0;
    public static float Default_Base_CritChance = 0;
    public static float Default_Base_CritDmgBounus = 200;
    public static float Default_Base_LPH = 0;
    public static float Default_Base_ManaRegen = 10;


    public static float Health_Weight = 10f;
    public static float Mana_Weight = 0.4f;
    public static float AD_Weight = 1f;
    public static float MD_Weight = 1f;
    public static float AttkSpd_Weight = 0.25f;
    public static float MoveSpd_Weight = 0.25f;
    public static float Defense_Weight = 0.25f;
    public static float CritChance_Weight = 0.25f;
    public static float CritDmgBounus_Weight = 2.5f;
    public static float LPH_Weight = 1f;
    public static float ManaRegen_Weight = 0.1f;
}
