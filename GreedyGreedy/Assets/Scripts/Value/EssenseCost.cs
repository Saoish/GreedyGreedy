using UnityEngine;
using System.Collections;

public class EssenseCost : Value {
    public EssenseCost(float Amount, ObjectController Source, System.Type Type) {
        this.Amount = Amount;
        this.Source = Source;
        this.Type = Type;
    }

}
