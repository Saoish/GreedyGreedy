using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Path_L : MonoBehaviour {
    MainPlayer MPC;

    Text PathInfo;

    int[] PathContainSkills = new int[] { 0, 1, 2, 3, 4, 5, 6, 7,8 };
	// Use this for initialization
	void Start () {
        MPC = transform.parent.GetComponent<Tab_1>().MPC;
        PathInfo = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePathInfo();

    }

    int GetPathTotal() {
        int total = 0;
        foreach (int skillindex in PathContainSkills) {
            total += MPC.GetSkilllvlByIndex(skillindex);
        }
        return total;
    }

    void UpdatePathInfo() {
        if (MPC.GetClass() == "Warrior") {
            PathInfo.color = MyColor.Orange;
            if (GetPathTotal() != 0)
                PathInfo.text = "Berserker (" + GetPathTotal() + ")";
            else
                PathInfo.text = "Berserker";
        }
    }

}
