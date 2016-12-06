using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class Stats{
    public static int Size = 13;

    public float[] stats;

    public Stats(InitStatsType type = InitStatsType.CHAR) {
        switch (type) {
            case InitStatsType.CHAR:
                stats = new float[] {
                100, //HEALTH
                100, //MANA
                0,   //AD
                0,   //MD
                0,   //DEFENSE
                100, //ATTACK_SPEED
                100, //MOVE_SPEED
                0,   //CRIT_CHANCE
                200, //CRIT_DMG
                0,   //LPH
                0,   //CDR
                0,   //HEALTH_REGEN
                10   //MANA_REGEN
                };
                break;
            case InitStatsType.EQUIP:
                stats = new float[13];
                break;
        } 
    }

    public void Set(StatsType type, float value) {
        stats[(int)type] = value;
    }

    public void Set(int type, float value) {
        stats[type] = value;
    }

    public float Get(StatsType type) {
        return stats[(int)type];
    }

    public float Get(int type) {
        return stats[type];
    }

    public void Add(StatsType type,float value) {
        stats[(int)type] += value;
    }

    public void Add(int type, float value) {
        stats[type] += value;
    }

    public void Dec(StatsType type, float value) {
        if (stats[(int)type] - value >= 0)
            stats[(int)type] -= value;
        else
            stats[(int)type] = 0;
    }

    public void Dec(int type, float value) {
        if (stats[type] - value >= 0)
            stats[type] -= value;
        else
            stats[type] = 0;
    }
    
}
