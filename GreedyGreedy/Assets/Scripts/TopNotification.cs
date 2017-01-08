using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopNotification : MonoBehaviour {
    static Text message;
    static Animator Anim;

    static GameObject self;
    
    void Awake() {
        Anim = GetComponent<Animator>();
        message = transform.Find("Message").GetComponent<Text>();
        self = gameObject;
    }

    public static void Push(string message, Color color, float period) {
        TopNotification.message.color = color;
        TopNotification.message.text = message;
        GameManager.instance.StartCoroutine(Transition(period));
    }

    public static string Message {
        get { return message.text; }
        set { message.text = value; }
    }   

    static IEnumerator Transition(float period) {
        Anim.SetBool("Off", false);
        Anim.SetBool("On",true);
        yield return new WaitForSeconds(period);
        if (self != null) {
            Anim.SetBool("Off", true);
            Anim.SetBool("On", false);
        }
    }


}
