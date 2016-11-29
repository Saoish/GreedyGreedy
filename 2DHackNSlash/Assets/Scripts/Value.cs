using UnityEngine;
using System.Collections;

public class Value : ScriptableObject {
    public ObjectController SourceOC = null;
    public float Amount = 0;
    public int Type = 0; //0->direct damage, 1->healing, -1-> dot damage type
    public bool IsCrit = false;

    public static Value CreateValue(float Amount = 0, int Type = 0, bool IsCrit = false, ObjectController source = null) {
        Value v = ScriptableObject.CreateInstance<Value>();
        v.Amount = Amount;
        v.Type = Type;
        v.IsCrit = IsCrit;
        v.SourceOC = source;
        return v;
    }
}
