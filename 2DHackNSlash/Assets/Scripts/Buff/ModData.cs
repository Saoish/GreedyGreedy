using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ModData : ScriptableObject {
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public float Duration;
    [HideInInspector]
    public float ModHealth = 0;
    [HideInInspector]
    public float ModMana = 0;
    [HideInInspector]
    public float ModAD = 0;
    [HideInInspector]
    public float ModMD = 0;
    [HideInInspector]
    public float ModAttSpd = 0;
    [HideInInspector]
    public float ModMoveSpd = 0;
    [HideInInspector]
    public float ModModfense = 0;
    [HideInInspector]
    public float ModCritChance = 0;
    [HideInInspector]
    public float ModCritDmgBounus = 0;
    [HideInInspector]
    public float ModLPH = 0;
    [HideInInspector]
    public float ModMPH = 0;
}
