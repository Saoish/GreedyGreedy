using UnityEngine;
using System.Collections;

public class HealHP : Value {
    public HealHP(float RawHeal, bool Crit, ObjectController Source, System.Type Type) {
        this.Amount = HealHPCalculation(RawHeal);
        this.Crit = Crit;
        this.Source = Source;
        this.Type = Type;
    }

    private float HealHPCalculation(float RawHeal) {
        return Mathf.Ceil(RawHeal);
    }
}
