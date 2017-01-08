using UnityEngine;
using System.Collections;
using GreedyScene;
using Networking;

public class BackButton : MonoBehaviour {

    public void GoBack() {
        if(Scene.Current_Int - 1 <=0)
            Client.ManuallyDisconnectSelf();
        Scene.Load(Scene.Current_Int - 1);
    }

}
