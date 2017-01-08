using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractionNotification : MonoBehaviour {
    static Text message;
    //public static bool Loaded = false;
    
    void Awake() {
        message = GetComponentInChildren<Text>(true);
        //Loaded = true;
    }

    public static string Message {
        get { return message.text; }
        set {  message.text = value; }
    }

    public static void TurnOn() {
        message.gameObject.SetActive(true);
    }

    public static void TurnOff() {
        message.gameObject.SetActive(false);        
    }

}
