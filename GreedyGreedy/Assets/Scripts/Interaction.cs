using UnityEngine;
using System.Collections;

public abstract class Interaction : MonoBehaviour {

    protected virtual void Awake() {
        gameObject.layer = LayerMask.NameToLayer(CollisionLayer.Interaction);
    }

    public abstract void Interact();
    public abstract void Disengage();

    public abstract void SetMessage();

    public static void TurnOffAllInteractionWindow() {
        foreach (InteractionContent IC in FindObjectsOfType<InteractionContent>()) {
            IC.TurnOff();
        }
    }
}
