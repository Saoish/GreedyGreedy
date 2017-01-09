using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RootButtons : MonoBehaviour {
    public static CanvasGroup cg;

    void Awake() {
        cg = GetComponent<CanvasGroup>();
    }    
}
