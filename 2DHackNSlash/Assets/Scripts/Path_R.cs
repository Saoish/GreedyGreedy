using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;

public class Path_R : MonoBehaviour {

    MainPlayer MPC;

    Text PathInfo;

    int[] PathContainSkills = new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17 };
    // Use this for initialization
    void Start() {
        MPC = transform.parent.GetComponent<Tab_1>().MPC;
        PathInfo = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
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
        if (MPC.GetClass() == Class.Warrior) {
            PathInfo.color = MyColor.Grey;
            if (GetPathTotal() != 0)
                PathInfo.text = "Mountain (" + GetPathTotal() + ")";
            else
                PathInfo.text = "Mountain";
        }
    }
}
