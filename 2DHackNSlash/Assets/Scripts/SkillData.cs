using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillData{
    public string Name;
    public string Description = "";
    public int lvl = 0;

    public SkillData() {

    }

    public SkillData(string Name) {
        this.Name = Name;
    }
}
