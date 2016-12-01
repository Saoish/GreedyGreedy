using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static float SFX_Volume = 1;
    public static int Show_Names = 1;

    public static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void OnLevelWasLoaded() {
        
    }

    void Update() {
        if (Input.GetKeyDown(ControllerManager.ToggleShow) || Input.GetAxisRaw(ControllerManager.J_DH)<0) {
            Show_Names *= -1;
        }
    }

    public void LoadSelectionScene() {
        Application.LoadLevel("Selection");
    }

    public void Exit() {
        Application.Quit();
    }

    public void LoadMenuScene() {
        Application.LoadLevel("Menu");
    }

    public static void LoadScene(string SceneName) {
        Application.LoadLevel(SceneName);
    }
    
    public static void LoadSceneWithWaitTime(string SceneName, float time) {
        instance.StartCoroutine(LoadSceneAfter(SceneName,time));
    }

    static IEnumerator LoadSceneAfter(string SceneName, float time) {
        yield return new WaitForSeconds(time);
        LoadScene(SceneName);
    }
}
