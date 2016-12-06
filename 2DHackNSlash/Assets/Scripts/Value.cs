using UnityEngine;
using System.Collections;

public class Value{
    public ObjectController SourceOC = null;
    public float Amount = 0;
    public int Type = 0; //0->direct damage, 1->healing
    public bool IsCrit = false;
    public bool SFX_Update = true;
    public bool Pop_Update = true;

    public Value(float Amount = 0, int Type = 0, bool IsCrit = false, ObjectController Source = null, bool SFX_Update = true, bool Pop_Update = true) {
        this.Amount = Amount;
        this.Type = Type;
        this.IsCrit = IsCrit;
        this.SFX_Update = SFX_Update;
        this.Pop_Update = Pop_Update;
        this.SourceOC = Source;
    }
}
