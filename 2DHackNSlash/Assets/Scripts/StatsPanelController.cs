using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StatsPanelController : MonoBehaviour {

    public string Percision = "F1";

    Text Stats;

    Text HealthValue;
    Text ManaValue;
    Text ADValue;
    Text MDValue;
    Text AttkSpdValue;
    Text MoveSpdValue;
    Text DefenseValue;
    Text CritChanceValue;
    Text CritBonusValue;
    Text LifePerHitValue;
    Text ManaRegenValue;

    MainPlayer MPC;

    // Use this for initialization
    void Start () {
        MPC = transform.parent.GetComponent<Tab_0>().MPC;

        Stats = transform.Find("Stats").GetComponent<Text>();

        HealthValue = transform.Find("Values/Health Value").GetComponent<Text>();
        ManaValue = transform.Find("Values/Mana Value").GetComponent<Text>();
        ADValue = transform.Find("Values/Attack Dmg Value").GetComponent<Text>();
        MDValue = transform.Find("Values/Magic Dmg Value").GetComponent<Text>();
        AttkSpdValue = transform.Find("Values/Attack Speed Value").GetComponent<Text>();
        MoveSpdValue = transform.Find("Values/Move Speed Value").GetComponent<Text>();
        DefenseValue = transform.Find("Values/Defense Value").GetComponent<Text>();
        CritChanceValue = transform.Find("Values/Crit Chance Value").GetComponent<Text>();
        CritBonusValue = transform.Find("Values/Crit Bonus Value").GetComponent<Text>();
        LifePerHitValue = transform.Find("Values/Life Per Hit Value").GetComponent<Text>();
        ManaRegenValue = transform.Find("Values/Mana Regen Value").GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        UpdateStatsUI();
	}

    void UpdateStatsUI() {
        Stats.text = "Lvl " + MPC.Getlvl() + " : " + MPC.GetExp() + "/" + MPC.GetNextLvlExp();//Just for now

        //Stats update
        HealthValue.text = MPC.GetCurrHealth().ToString(Percision);
        ManaValue.text = MPC.GetCurrMana().ToString(Percision);
        ADValue.text = MPC.GetCurrAD().ToString(Percision);
        MDValue.text = MPC.GetCurrMD().ToString(Percision);
        AttkSpdValue.text = MPC.GetCurrAttkSpd().ToString(Percision) + "%";
        MoveSpdValue.text = MPC.GetCurrMoveSpd().ToString(Percision) + "%";
        DefenseValue.text = MPC.GetCurrDefense().ToString(Percision) + "%";
        CritChanceValue.text = MPC.GetCurrCritChance().ToString(Percision) + "%";
        CritBonusValue.text = MPC.GetCurrCritDmgBounus().ToString(Percision) + "%";
        LifePerHitValue.text = MPC.GetCurrLPH().ToString(Percision);
        ManaRegenValue.text = MPC.GetCurrManaRegen().ToString(Percision)+"/s";

        //Gear Update



        //Invetory Update



        //Talents Update




    }
}
