using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillData : ScriptableObject {
    public string Name;
    public string Description = "";
    public int lvl = 0;

    public static SkillData CreateSkillData(string Name, string Description, int lvl) {
        SkillData SD = CreateInstance<SkillData>();
        SD.Name = Name;
        SD.lvl = lvl;
        SD.Description = Description;
        return SD;
    }
}
