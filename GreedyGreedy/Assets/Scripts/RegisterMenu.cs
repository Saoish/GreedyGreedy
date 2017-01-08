using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Networking;
using Networking.Data;

public class RegisterMenu : MonoBehaviour {
    public PopUpNotification PopUp;
    public GameObject RootButtonsOJ;
    public GameObject LoginButtonOJ;
    public GameObject RegisterButtonOJ;
    public GameObject BackButtonOJ;
    public InputField Username;
    //public InputField Password;    

    public void Register() {//This function will be disable after hook up with Steam login
        if (Username.text == "" /*|| Password.text == ""*/) {
            PopUpNotification.Push("Username is invalid.", PopUpNotification.Type.Confirm);
        }
        else {
            PopUpNotification.Push("Waiting for server...", PopUpNotification.Type.Pending);
            Client.Connect();
            StartCoroutine(CheckConnectionAndSendRequest(2));
        }
    }

    public void EnableForRegisteration() {        
        Username.text = "";
        //Password.text = "";
        RootButtonsOJ.SetActive(false);
        LoginButtonOJ.SetActive(false);
        RegisterButtonOJ.SetActive(true);
        gameObject.SetActive(true);
    }

    //public void EnableForLogin() {
    //    Username.text = "";
    //    //Password.text = "";
    //    RootButtonsOJ.SetActive(false);
    //    LoginButtonOJ.SetActive(true);
    //    RegisterButtonOJ.SetActive(false);
    //    gameObject.SetActive(true);
    //}

    public void Disable() {        
        gameObject.SetActive(false);
        RootButtonsOJ.SetActive(true);
    }

    //public void EnableInteraction() {
    //    GetComponent<CanvasGroup>().interactable = true;
    //}
    //public void DisableInteraction() {
    //    GetComponent<CanvasGroup>().interactable = false;
    //}
    

    private IEnumerator CheckConnectionAndSendRequest(float time) {                       
        yield return new WaitForSeconds(time);
        if (!Client.Connected) {
            PopUpNotification.Push("No connection to server.", PopUpNotification.Type.Confirm);
        } else {            
            Client.Send(Protocols.RegisterUser, Username.text);
        }
    }
}
