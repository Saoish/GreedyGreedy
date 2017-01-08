using UnityEngine;
using System.Collections;

public abstract class InteractionContent : MonoBehaviour {

    protected abstract void OnEnable();
    protected abstract void OnDisable();

    public void TurnOn() {
        gameObject.SetActive(true);
    }

    public void TurnOff() {
        gameObject.SetActive(false);
    }
}
