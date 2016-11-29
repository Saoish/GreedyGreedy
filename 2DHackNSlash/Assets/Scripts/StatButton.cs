using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatButton : MonoBehaviour {

    MainPlayer MPC;

    public AudioClip stat_lvlup;

    public AudioClip selected;

    Animator Anim;

    string Stat;

    public void OnSelect() {
        AudioSource.PlayClipAtPoint(selected, transform.position, GameManager.SFX_Volume);
    }

    // Use this for initialization
    void Start () {
        Stat = gameObject.name;
        Anim = GetComponent<Animator>();
        MPC = transform.parent.parent.parent.GetComponent<CharacterSheetController>().MPC;
    }
	
	// Update is called once per frame
	void Update () {
        if (MPC.GetAvailableStatPoints() != 0) {
            transform.GetComponent<Image>().color = MyColor.Yellow;
            var colors = transform.GetComponent<Button>().colors;
            colors.highlightedColor = MyColor.Green;
            transform.GetComponent<Button>().colors = colors;
            Anim.enabled = true;
            //Anim.SetBool("Blinking", true);
        } else {
            //Anim.SetBool("Blinking", false);
            Anim.enabled = false;
            transform.GetComponent<Image>().color = MyColor.Grey;
            var colors = transform.GetComponent<Button>().colors;
            colors.highlightedColor = MyColor.Red;
            transform.GetComponent<Button>().colors = colors;
        }
    }

    public void AddStatPoint() {
        if (MPC.GetAvailableStatPoints() <= 0) {
            RedNotification.Push(RedNotification.Type.NO_STAT_POINT);
        } else {
            MPC.AddStatPoint(Stat);
            AudioSource.PlayClipAtPoint(stat_lvlup, transform.position, GameManager.SFX_Volume);
        }
    }

}
