using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyScene;

public class LoadingUI : MonoBehaviour {

    static Text _LoadingText;
    static Image _FillBar;
    static Text _Progress;

    void Awake() {
        _LoadingText = transform.Find("LoadingText").GetComponent<Text>();
        _FillBar = transform.Find("ProgressBar/FillBar").GetComponent<Image>();
        _Progress = transform.Find("ProgressBar/Text").GetComponent<Text>();
        Initialize();
    }

    void Update() {
        SyncProgress();
    }

    private void SyncProgress() {
        if (Scene.async == null)
            return;
        FillBar.fillAmount = 0.1f+Scene.async.progress;
        Progress = (10+Scene.async.progress * 100).ToString("F0") + "%";
    }

    public static string LoadingText {
        get { return _LoadingText.text; }
        set { _LoadingText.text = value; }
    }

    public static Image FillBar {
        get { return _FillBar; }
        set { _FillBar = value; }
    }

    public static string Progress {
        get { return _Progress.text; }
        set { _Progress.text = value; }
    }

    public static void Initialize() {        
        _LoadingText.text = "Loading...";
        _FillBar.fillAmount = 0;
        _Progress.text = "0%";        
    }
    
}
