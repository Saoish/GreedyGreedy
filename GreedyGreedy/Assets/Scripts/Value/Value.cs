using UnityEngine;
using System.Collections;

public abstract class Value {
    public float Amount = 0;
    public bool Crit = false;
    public ObjectController Source = null;
    public System.Type Type = null;
}
