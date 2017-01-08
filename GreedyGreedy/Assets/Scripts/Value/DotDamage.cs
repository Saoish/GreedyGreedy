using UnityEngine;
using System.Collections;

public class DotDamage : Damage {
    public DotDamage(float RawDamage,bool Crit, ObjectController Source, System.Type Type) {
        this.Amount = DotDamageCalculation(RawDamage);
        this.Crit = Crit;
        this.Source = Source;
        this.Type = Type;
    }

    private float DotDamageCalculation(float RawDamage) {
        return Mathf.Ceil(RawDamage);
    }
}
