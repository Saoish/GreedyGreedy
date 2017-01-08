using UnityEngine;
using System.Collections;

public class BloodyHandIndicator : MonoBehaviour {

    [HideInInspector]
    public float Z_Angle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (ControllerManager.MoveVector != Vector2.zero)
            Z_Angle = Mathf.Atan2(ControllerManager.MoveVector.y, ControllerManager.MoveVector.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, Z_Angle);
    }

    public void Active() {
        Vector2 CastVector = Vector2.zero;
        if (ControllerManager.MoveVector != Vector2.zero) {
            CastVector = ControllerManager.MoveVector;
        } else {
            switch (ControllerManager.Direction) {
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
        Z_Angle = Mathf.Atan2(CastVector.y, CastVector.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, Z_Angle);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void Deactive() {
        gameObject.SetActive(false);
    }
}
