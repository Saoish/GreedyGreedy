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
    Text ManaPerHitValue;

    PlayerController PC;

    // Use this for initialization
    void Start () {
        PC = GameObject.Find("MainPlayer/PlayerController").transform.GetComponent<PlayerController>();

        Stats = transform.Find("Stats").GetComponent<Text>();

        HealthValue = transform.Find("Health Value").GetComponent<Text>();
        ManaValue = transform.Find("Mana Value").GetComponent<Text>();
        ADValue = transform.Find("Attack Dmg Value").GetComponent<Text>();
        MDValue = transform.Find("Magic Dmg Value").GetComponent<Text>();
        AttkSpdValue = transform.Find("Attack Speed Value").GetComponent<Text>();
        MoveSpdValue = transform.Find("Move Speed Value").GetComponent<Text>();
        DefenseValue = transform.Find("Defense Value").GetComponent<Text>();
        CritBonusValue = transform.Find("Crit Chance Value").GetComponent<Text>();
        CritChanceValue = transform.Find("Crit Bonus Value").GetComponent<Text>();
        LifePerHitValue = transform.Find("Life Per Hit Value").GetComponent<Text>();
        ManaPerHitValue = transform.Find("Mana Per Hit Value").GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        UpdateStatsUI();
	}

    void UpdateStatsUI() {
        Stats.text = "Lvl " + PC.Getlvl() + " : " + PC.GetExp() + "/" + PC.GetNextLvlExp();//Just for now

        //Stats update
        HealthValue.text = PC.GetCurrHealth().ToString(Percision);
        ManaValue.text = PC.GetCurrMana().ToString(Percision);
        ADValue.text = PC.GetCurrAD().ToString(Percision);
        MDValue.text = PC.GetCurrMD().ToString(Percision);
        AttkSpdValue.text = PC.GetCurrAttkSpd().ToString(Percision);
        MoveSpdValue.text = PC.GetCurrMoveSpd().ToString(Percision);
        DefenseValue.text = PC.GetCurrDefense().ToString(Percision);
        CritChanceValue.text = PC.GetCurrCritChance().ToString(Percision);
        CritBonusValue.text = PC.GetCurrCritDmgBounus().ToString(Percision);
        LifePerHitValue.text = PC.GetCurrLPH().ToString(Percision);
        ManaPerHitValue.text = PC.GetCurrMPH().ToString(Percision);

        //Gear Update



        //Invetory Update



        //Talents Update




    }
}
