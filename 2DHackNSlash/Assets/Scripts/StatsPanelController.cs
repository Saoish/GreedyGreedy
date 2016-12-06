using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GreedyNameSpace;
public class StatsPanelController : MonoBehaviour {
  
    public string Percision = "F1";

    Text Stats;

    Text HealthValue;
    Text ManaValue;
    Text ADValue;
    Text MDValue;
    Text DefenseValue;
    Text AttkSpdValue;
    Text MoveSpdValue;
    Text CritChanceValue;
    Text CritBonusValue;
    Text LifePerHitValue;
    Text HealthRegenValue;
    Text ManaRegenValue;
    Text CDRValue;

    MainPlayer MPC;

    // Use this for initialization
    void Start () {
        MPC = transform.parent.GetComponent<Tab_0>().MPC;

        Stats = transform.Find("Stats").GetComponent<Text>();

        HealthValue = transform.Find("Values/Health Value").GetComponent<Text>();
        ManaValue = transform.Find("Values/Mana Value").GetComponent<Text>();
        ADValue = transform.Find("Values/Attack Dmg Value").GetComponent<Text>();
        MDValue = transform.Find("Values/Magic Dmg Value").GetComponent<Text>();
        DefenseValue = transform.Find("Values/Defense Value").GetComponent<Text>();
        AttkSpdValue = transform.Find("Values/Attack Speed Value").GetComponent<Text>();
        MoveSpdValue = transform.Find("Values/Move Speed Value").GetComponent<Text>();
        CritChanceValue = transform.Find("Values/Crit Chance Value").GetComponent<Text>();
        CritBonusValue = transform.Find("Values/Crit Bonus Value").GetComponent<Text>();
        LifePerHitValue = transform.Find("Values/Life Per Hit Value").GetComponent<Text>();
        CDRValue = transform.Find("Values/CDR Value").GetComponent<Text>();
        HealthRegenValue = transform.Find("Values/Health Regen Value").GetComponent<Text>();
        ManaRegenValue = transform.Find("Values/Mana Regen Value").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateStatsUI();
	}

    void UpdateStatsUI() {
        Stats.text = "Lvl " + MPC.Getlvl() + " : " + MPC.GetExp() + "/" + MPC.GetNextLvlExp();//Just for now

        HealthValue.text = MPC.GetCurrStats(StatsType.HEALTH).ToString(Percision);
        ManaValue.text = MPC.GetCurrStats(StatsType.MANA).ToString(Percision);
        ADValue.text = MPC.GetCurrStats(StatsType.AD).ToString(Percision);
        MDValue.text = MPC.GetCurrStats(StatsType.MD).ToString(Percision);
        DefenseValue.text = MPC.GetCurrStats(StatsType.DEFENSE).ToString(Percision) + "%";
        AttkSpdValue.text = MPC.GetCurrStats(StatsType.ATTACK_SPEED).ToString(Percision) + "%";
        MoveSpdValue.text = MPC.GetCurrStats(StatsType.MOVE_SPEED).ToString(Percision) + "%";
        CritChanceValue.text = MPC.GetCurrStats(StatsType.CRIT_CHANCE).ToString(Percision) + "%";
        CritBonusValue.text = MPC.GetCurrStats(StatsType.CRIT_DMG).ToString(Percision) + "%";
        LifePerHitValue.text = MPC.GetCurrStats(StatsType.LPH).ToString(Percision) + "%";
        CDRValue.text = MPC.GetCurrStats(StatsType.CDR).ToString(Percision) + "%";
        HealthRegenValue.text = MPC.GetCurrStats(StatsType.HEALTH_REGEN).ToString(Percision) + "/s";
        ManaRegenValue.text = MPC.GetCurrStats(StatsType.MANA_REGEN).ToString(Percision) + "/s";



        //Gear Update



        //Invetory Update



        //Talents Update




    }
}
