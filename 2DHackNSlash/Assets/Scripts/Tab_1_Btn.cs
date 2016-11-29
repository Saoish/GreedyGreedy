using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Tab_1_Btn : MonoBehaviour {
    MainPlayer MPC;
    Animator Anim;
    Text SkillPoints_Text;
    // Use this for initialization
    void Start() {
        MPC = transform.parent.GetComponent<CharacterSheetController>().MPC;
        Anim = GetComponent<Animator>();
        SkillPoints_Text = transform.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (MPC.GetAvailableSkillPoints() != 0) {
            SkillPoints_Text.text = MPC.GetAvailableSkillPoints().ToString();
            Anim.SetBool("Blinking", true);
        } else {
            SkillPoints_Text.text = "";
            Anim.SetBool("Blinking", false);
        }

    }
}
