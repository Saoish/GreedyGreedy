using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSheetController : MonoBehaviour
{
    public string Percision = "F1";

    private PlayerController PC = null;

	public KeyCode ToggleOnOff = KeyCode.C;
	public KeyCode ToggleOff = KeyCode.Escape;

	public Light MouseLight = null;

	public GameObject ResourceLabel;
	public GameObject ResourcePerHitLabel;

	public GameObject HealthValue;
	public GameObject ResourceValue;
	public GameObject ADValue;
	public GameObject MDValue;
	public GameObject AttckSpdValue;
	public GameObject MoveSpdValue;
	public GameObject DefenseValue;
	public GameObject CritChanceValue;
	public GameObject CritBonusValue;
	public GameObject LifePerHitValue;
	public GameObject ResourcePerHitValue;

    GameObject ES;

    void Awake() {
        ES = GameObject.Find("EventSystem");
        PC = GameObject.Find("MainPlayer/PlayerController").transform.GetComponent<PlayerController>();
    }

	void Start ()
	{
		UpdateCharacterSheetUI ();

		if (PC.GetClass() == "Warrior") {
			ResourceLabel.GetComponent<UnityEngine.UI.Text> ().text = "Rage";
			ResourcePerHitLabel.GetComponent<UnityEngine.UI.Text> ().text = "Rage Per Hit";
		}
		else
		{
			ResourceLabel.GetComponent<UnityEngine.UI.Text> ().text = "Unknown";
			ResourcePerHitLabel.GetComponent<UnityEngine.UI.Text> ().text = "Unknown Per Hit";
		}
			
	}
	

	void Update ()
	{
        if (IsOn()) {
            UpdateCharacterSheetUI();
            Vector3 Mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Mouse = new Vector3(Mouse.x, Mouse.y, MouseLight.transform.position.z);
            MouseLight.transform.position = Mouse;
        }
	}

    public void Toggle() {
        if (IsOn()) {
            TurnOff();
        } else {
            TurnOn();
        }

    }

    public void TurnOn() {
        gameObject.SetActive(true);
        ES.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        GameObject FBO = GameObject.Find("MainPlayer/PlayerUI/CharacterSheet/InventoryButtons/0").gameObject;
        ES.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(FBO);
    }

    public void TurnOff() {
        gameObject.SetActive(false);
    }

    public bool IsOn() {
        return gameObject.active;
    }

    void UpdateCharacterSheetUI()
	{
        transform.Find("StatsPanel/Stats").GetComponent<Text>().text = "Lvl "+PC.Getlvl() + " : "+PC.GetExp() + "/" + PC.GetNextLvlExp();//Just for now

		//Stats update
		HealthValue.GetComponent<UnityEngine.UI.Text> ().text = 		PC.CurrHealth.ToString(Percision);
        ResourceValue.GetComponent<UnityEngine.UI.Text> ().text = 		PC.CurrMana.ToString(Percision);
        ADValue.GetComponent<UnityEngine.UI.Text> ().text = 			PC.CurrAD.ToString(Percision);
        MDValue.GetComponent<UnityEngine.UI.Text> ().text = 			PC.CurrMD.ToString(Percision);
        AttckSpdValue.GetComponent<UnityEngine.UI.Text> ().text = 		PC.CurrAttkSpd.ToString(Percision);
        MoveSpdValue.GetComponent<UnityEngine.UI.Text> ().text = 		PC.CurrMoveSpd.ToString(Percision);
        DefenseValue.GetComponent<UnityEngine.UI.Text> ().text = 		PC.CurrDefense.ToString(Percision);
        CritChanceValue.GetComponent<UnityEngine.UI.Text> ().text =		PC.CurrCritChance.ToString(Percision);
        CritBonusValue.GetComponent<UnityEngine.UI.Text> ().text = 		PC.CurrCritDmgBounus.ToString(Percision);
        LifePerHitValue.GetComponent<UnityEngine.UI.Text> ().text = 	PC.CurrLPH.ToString(Percision);
        ResourcePerHitValue.GetComponent<UnityEngine.UI.Text> ().text = PC.CurrMPH.ToString(Percision);

        //Gear Update



        //Invetory Update



        //Talents Update




    }
}
