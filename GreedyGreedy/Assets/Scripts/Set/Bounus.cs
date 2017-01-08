using UnityEngine;
using System.Collections;
using GreedyNameSpace;

[System.Serializable]
public class Bounus{
    public BounusType bounus_type;
    public int condiction;
    public SetStatsField stats_bounus;
    public PassiveSkill passive_bounus;

    private PassiveSkill CachedPassive = null;

    public enum BounusType {
        Stats,
        Passive
    }
    public Bounus() {
        bounus_type = BounusType.Stats;
        condiction = 0;
    }

    public Bounus(Bounus b) {
        this.bounus_type = b.bounus_type;
        this.condiction = b.condiction;
        this.stats_bounus = b.stats_bounus;
        this.passive_bounus = b.passive_bounus;
        this.CachedPassive = null;
    }

    public void ApplyBounus(ObjectController target) {
        switch (bounus_type) {
            case BounusType.Stats:
                switch (stats_bounus.value_type) {
                    case SetStatsField.ValueType.Raw:
                        target.AddMaxStats(stats_bounus.stats_type, stats_bounus.value);
                        break;
                    case SetStatsField.ValueType.Percentage:
                        if ((int)STATSTYPE.DEFENSE <= (int)stats_bounus.stats_type && (int)stats_bounus.stats_type <= (int)STATSTYPE.HASTE)
                            target.AddMaxStats(stats_bounus.stats_type, (float)System.Math.Round(stats_bounus.value, 1));
                        else
                            target.AddMaxStats(stats_bounus.stats_type, (float)System.Math.Round(stats_bounus.value * target.MaxStats.Get(stats_bounus.stats_type), 0));
                        break;
                }
                break;
            case BounusType.Passive:
                if (target.GetPassive(passive_bounus.GetType()) != null)
                    return;
                CachedPassive = (PassiveSkill)passive_bounus.Instantiate();
                CachedPassive.InitSkill(target);
                target.Passives.Add(CachedPassive);
                break;
        }
    }

    public void RemoveBounus() {
        switch (bounus_type) {
            case BounusType.Stats:
                return;
            case BounusType.Passive:
                if (CachedPassive != null) {
                    CachedPassive.GetOC().Passives.Remove(CachedPassive);
                    CachedPassive.Delete();
                }
                break;
        }
    }
}
