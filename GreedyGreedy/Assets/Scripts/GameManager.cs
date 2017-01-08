using UnityEngine;
using System.Collections;
using Networking;

public class GameManager : MonoBehaviour {
    public static float SFX_Volume = 1;
    public static int Show_Names = 1;

    public static GameManager instance;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    //void OnLevelWasLoaded() {
        
    //}

    void Update() {
        if (ControllerManager.SyncActions && ControllerManager.Actions.ToggleName.WasPressed) {
            Show_Names *= -1;
        }
    }

    public void Exit() {
        Client.ShutDown();        
        Application.Quit();
    }


    //public static void LoadScene(string SceneName) {
    //    Application.LoadLevel(SceneName);
    //}
    
    //public static void LoadSceneWithWaitTime(string SceneName, float time) {
    //    instance.StartCoroutine(LoadSceneAfter(SceneName,time));
    //}

    //static IEnumerator LoadSceneAfter(string SceneName, float time) {
    //    yield return new WaitForSeconds(time);
    //    LoadScene(SceneName);
    //}
}
