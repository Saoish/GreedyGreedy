using UnityEngine;
using System.Collections;
using InControl;

public class ActionsAdapter : MonoBehaviour {
    InControlInputModule ICIM;

    //public static EventSystemManager instance;

    void Awake() {
        ICIM = GetComponent<InControlInputModule>();
    }

    void OnEnable() {
        if (ICIM != null) {
            ICIM.SubmitAction = ControllerManager.Actions.Submit;
            ICIM.CancelAction = ControllerManager.Actions.Cancel;
            ICIM.MoveAction = ControllerManager.Actions.Move;
        }
    }

}
