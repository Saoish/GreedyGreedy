using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

public class SkillInfo : MonoBehaviour {
    Text Name;
    Image SkillIcon;
    Text Type;
    Text Level;
    Text Requirement;
    Text Description;

    void OnEnable() {
        Name = transform.Find("Title/Name").GetComponent<Text>();
        SkillIcon = transform.Find("Prefix/SkillIcon").GetComponent<Image>();
        Type = transform.Find("Prefix/Type").GetComponent<Text>();
        Requirement = transform.Find("Prefix/Requirement").GetComponent<Text>();
        Description = transform.Find("Description/Text").GetComponent<Text>();
    }

    public void Show(Skill S,string PathName, bool Satisfied,int RequiredPoints) {
        gameObject.SetActive(true);
        SetName(S);
        SetSkillIcon(S);
        SetType(S);
        //SetLevel(S);
        SetRequirement(PathName, Satisfied, RequiredPoints);
        SetDescription(S);
    }

    void SetName(Skill S) {
        Name.text = S.Name;
    }

    void SetSkillIcon(Skill S) {
        SkillIcon.sprite = Resources.Load<Sprite>("SkillIcons/" + S.Name);
    }

    void SetType(Skill S) {
        if (S.GetType().IsSubclassOf(typeof(ActiveSkill))) {
            Type.color = MyColor.Orange;
            Type.text = "Active";
        } else {
            Type.color = MyColor.Grey;
            Type.text = "Passive";
        }
    }

    //void SetLevel(Skill S) {
    //    if (S.lvl == 0) {
    //        Level.color = MyColor.Red;
    //        Level.text = "Not yet learned";
    //    } else if (S.lvl == Patch.MaxSkilllvl) {
    //        Level.color = MyColor.Purple;
    //        Level.text = "Level Max";
    //    } else {
    //        Level.color = MyColor.Orange;
    //        Level.text = "Level " + S.lvl;
    //    }
    //}

    void SetRequirement(string PathName, bool Satisfied, int RequiredPoints) {
        if (Satisfied) {
            Requirement.fontSize = 0;
            Requirement.text = "";
        } else {
            Requirement.fontSize = 40;
            Requirement.color = MyColor.Red;
            Requirement.text = "(Required " + PathName + " " + RequiredPoints + ")";
        }
        
    }

    void SetDescription(Skill S) {
        Description.text = S.Description;
    }
}
