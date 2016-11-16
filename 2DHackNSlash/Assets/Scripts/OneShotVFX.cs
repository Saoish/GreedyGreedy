using UnityEngine;
using System.Collections;

public class OneShotVFX : MonoBehaviour {

    public AudioClip SFX;

    void Start() {
        if (SFX)
            AudioSource.PlayClipAtPoint(SFX, transform.position, GameManager.SFX_Volume);
    }
}
