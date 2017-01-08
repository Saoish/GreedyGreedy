using UnityEngine;
using System.Collections;
using Networking;
using Networking.Data;

public class LoginButton : MonoBehaviour {
    public PopUpNotification PopUp;
    public void Login() {
        if (DataManager.Username == null) {
            PopUpNotification.Push("You have not yet registered.", PopUpNotification.Type.Confirm);
        } else {
            PopUpNotification.Push("Waiting for server...", PopUpNotification.Type.Pending);
            Client.Connect();
            StartCoroutine(CheckConnectionAndSendRequest(0.5f));
        }
    }

    private IEnumerator CheckConnectionAndSendRequest(float time) {
        yield return new WaitForSeconds(time);
        if (!Client.Connected) {
            PopUpNotification.Push("No connection to server.", PopUpNotification.Type.Confirm);
        } else {
            Client.Send(Protocols.UserLogin, DataManager.Username);
        }
    }
}
