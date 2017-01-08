using UnityEngine;
using System.Collections;

public class WarStompIndicator : MonoBehaviour {
    float GrowingRate = 1f;    
    float ScalingCap = 2;
    [HideInInspector]
    public float ScalingFactor = 1;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (ScalingFactor < ScalingCap) {
            ScalingFactor += GrowingRate * Time.deltaTime;
        } else {
            ScalingFactor = ScalingCap;
        }
        transform.localScale = new Vector3(ScalingFactor, ScalingFactor, 1);
    }

    public void Active() {
        ScalingFactor = 1;
        gameObject.SetActive(true);
    }

    public void Deactive() {
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.SetActive(false);
    }
}
