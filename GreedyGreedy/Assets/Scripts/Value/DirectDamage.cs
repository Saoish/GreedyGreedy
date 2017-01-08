using UnityEngine;
using System.Collections;

public class DirectDamage : Damage {

    public DirectDamage(float RawDamage,float TargetDefense, float Penetration, bool Crit, ObjectController Source, System.Type Type) {
        this.Amount = DirectDamageCalculation(RawDamage, TargetDefense, Penetration);
        this.Crit = Crit;
        this.Source = Source;
        this.Type = Type;        
    }

    private float DirectDamageCalculation(float RawDamage, float TargetDefense, float Penetration) {
        float Diff = TargetDefense - Penetration;
        if (Diff < 0) {
            Diff = 0;
        }
        float reduced_dmg = RawDamage * Diff / 100;
        return RawDamage - reduced_dmg >= 1 ? Mathf.Ceil(RawDamage - reduced_dmg) : 1;
    }
}
