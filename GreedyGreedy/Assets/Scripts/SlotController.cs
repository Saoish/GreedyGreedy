using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using GreedyNameSpace;
using GreedyScene;
using Networking;
using Networking.Data;
public class SlotController : MonoBehaviour {
    public Transform VisualHolder;

    int Slot;

    Text Name;
    Text LvlClass;

    GameObject PlayButtonObject;
    GameObject DeleteButtonObject;
    GameObject CreateButtonObject;
        
    PlayerData PlayerData;

    private GameObject BaseModel;

    void Awake() {
        Slot = int.Parse(gameObject.name);
        Name = transform.Find("NameText").GetComponent<Text>();
        LvlClass = transform.Find("LvlClassText").GetComponent<Text>();

        PlayButtonObject = transform.Find("PlayButton").gameObject;
        DeleteButtonObject = transform.Find("DeleteButton").gameObject;
        CreateButtonObject = transform.Find("CreateButton").gameObject;

        PlayerData = DataManager.UserData.PlayerDatas[Slot];
    }
            
    // Use this for initialization
	void Start () {
        LoadSlotData();
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void PlayButtonOnClick() {
        CacheManager.CachedPlayerSlotIndex = Slot;
        PopUpNotification.Push("Waiting for server...");
        Client.Send(Protocols.SubscribeIdentityAndInstance,Slot);
    }

    public void CreateButtonOnClick() {
        CacheManager.CachedPlayerSlotIndex = Slot;
        Scene.Load(ID.CharacterCreation);  
    }

    public void DeleteButtonOnClick() {
        PopUpNotification.Push("Are you sure?", PopUpNotification.Type.Select);
        StartCoroutine(WaitFroDeleteDecision());
    }

    void LoadSlotData() {
        if(PlayerData.Name == "") {
            Name.text = LvlClass.text = "";
            PlayButtonObject.SetActive(false);
            DeleteButtonObject.SetActive(false);
            CreateButtonObject.SetActive(true);
        } else {
            Name.text = PlayerData.Name;
            LvlClass.text = "lvl " + PlayerData.lvl + " " + PlayerData.Class;
            CreateButtonObject.SetActive(false);
            InstaniateEquipment();
        }
            
    }

    IEnumerator WaitFroDeleteDecision() {
        PopUpNotification.Push("Are you sure?", PopUpNotification.Type.Select);
        yield return PopUpNotification.WaitForDecision();
        if (PopUpNotification.Decision) {            
            Client.Send(Protocols.DeleteCharacter, new DeletionData(Slot));
            PopUpNotification.Push("Waiting for server...");
        }
    }






























    void InstaniateEquipment() {
        BaseModel = Instantiate(Resources.Load("BaseModelPrefabs/BaseModel"), VisualHolder) as GameObject;
        BaseModel.name = "BaseModel";
        BaseModel.GetComponent<SpriteRenderer>().color = new Color(PlayerData.SkinColor.R, PlayerData.SkinColor.G, PlayerData.SkinColor.B);
        BaseModel.transform.localPosition = Vector3.zero;
        foreach (var e in PlayerData.Equipments) {
            if (e.Name!= "") {
                GameObject equipPrefab = EquipmentController.ObtainPrefabForCharacterSelection(e, VisualHolder);
            }
        }
    }
}