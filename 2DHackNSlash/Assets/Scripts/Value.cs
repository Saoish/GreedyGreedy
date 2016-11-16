using UnityEngine;
using System.Collections;

public class Value : ScriptableObject {
    public float Amount = 0;
    public int Type = 0; //0->Subtrative, 1->Addictive
    public bool IsCrit = false;

    public static Value CreateValue(float Amount = 0, int Type = 0, bool IsCrit = false) {
        Value v = ScriptableObject.CreateInstance<Value>();
        v.Amount = Amount;
        v.Type = Type;
        v.IsCrit = IsCrit;
        return v;
    }
}
