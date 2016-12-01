using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tab_0_Btn : MonoBehaviour {
    MainPlayer MPC;
    Animator Anim;
    Text StatPoints_Text;
	// Use this for initialization
	void Start () {
        MPC = transform.parent.GetComponent<CharacterSheetController>().MPC;
        Anim = GetComponent<Animator>();
        StatPoints_Text = transform.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (MPC.GetAvailableStatPoints() != 0) {
            StatPoints_Text.text = MPC.GetAvailableStatPoints().ToString();
            Anim.enabled = true;
            Anim.SetBool("Blinking", true);
        } else {
            StatPoints_Text.text = "";
            Anim.SetBool("Blinking", false);
            Anim.enabled = false;
            Color c = GetComponent<Image>().color;
            c.a = 0;
            GetComponent<Image>().color = c;
        }

    }

    
}
