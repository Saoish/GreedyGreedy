using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;
public class StatsPanelController : MonoBehaviour {
  
    public string Percision = "F1";

    Text Stats;

    Text[] Labels;
    Text[] Values;

    MainPlayer MPC;

    // Use this for initialization
    void Start () {
        MPC = transform.parent.GetComponent<Tab_0>().MPC;

        Stats = transform.Find("Stats").GetComponent<Text>();
        Labels = transform.Find("Labels").GetComponentsInChildren<Text>();
        Values = transform.Find("Values").GetComponentsInChildren<Text>();
        for (int i = 0; i < Labels.Length; i++) {
            Labels[i].text = StatsType.GetStatsTypeString(i).F;
        }

    }
	
	// Update is called once per frame
	void Update () {
        UpdateStatsUI();
	}

    void UpdateStatsUI() {
        Stats.text = "Lvl " + MPC.Getlvl() + " : " + MPC.GetExp() + "/" + MPC.GetNextLvlExp();//Just for now
        for(int i = 0; i < Values.Length; i++) {
            Values[i].text = MPC.GetCurrStats((STATSTYPE)i) + StatsType.GetStatsTypeString(i).S;
        }
    }

}
