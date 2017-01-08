using UnityEngine;
using System.Collections;

public class PhantomChargeIndicator : MonoBehaviour {
    [HideInInspector]
    public Vector2 CastVector;

    void Update() {
        if (ControllerManager.MoveVector != Vector2.zero) {
            CastVector = ControllerManager.MoveVector;            
        }
        float AngleZ = Mathf.Atan2(CastVector.y, CastVector.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, AngleZ);
    }

    public void Active(int Direction) {
        if (ControllerManager.MoveVector != Vector2.zero) {
            CastVector = ControllerManager.MoveVector;
        } else {
            switch (Direction) {
                case 0:
                    CastVector = new Vector2(0, -1);
                    break;
                case 1:
                    CastVector = new Vector2(-1, 0);
                    break;
                case 2:
                    CastVector = new Vector2(1, 0);
                    break;
                case 3:
                    CastVector = new Vector2(0, 1);
                    break;
            }
        }
        float Z_Angle = Mathf.Atan2(CastVector.y, CastVector.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, Z_Angle);
        gameObject.SetActive(true);
    }
    public void Deactive() {
        gameObject.SetActive(false);
    }
}
