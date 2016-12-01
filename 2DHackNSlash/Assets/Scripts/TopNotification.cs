using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopNotification : MonoBehaviour {
    static Text Message;
    static RectTransform BG_T;
    static Animator Anim;
    
    void Awake() {
        Anim = GetComponent<Animator>();
        BG_T = GetComponent<RectTransform>();
        Message = transform.Find("Message").GetComponent<Text>();
    }

    public static void Push(string message, Color color) {
        BG_T.sizeDelta = new Vector2(60 * message.Length, BG_T.rect.height);
        Message.color = color;
        Message.text = message;
        Anim.Play("display",0,0);
    }
}
