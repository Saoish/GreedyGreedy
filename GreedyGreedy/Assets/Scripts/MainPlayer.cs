using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using GreedyNameSpace;
using GreedyScene;
using Networking;
using Networking.Data;

public class MainPlayer : Player {
    public ContactDetector ContactDetector;
    [HideInInspector]
    public Interaction InteractTarget = null;
    [HideInInspector]
    public MainPlayerUI MPUI;

    Camera MainCamera;

    private Vector2 FilteredMoveVector;

    public static void Instantiate(int ClientID,PlayerData PlayerData, Vector2 Position) {
        GameObject PlayerOJ = Resources.Load("PlayerPrefabs/MainPlayer") as GameObject;
        PlayerOJ.GetComponent<MainPlayer>().PlayerData = PlayerData;
        PlayerOJ = Instantiate(PlayerOJ, Position, Quaternion.identity) as GameObject;
        PlayerOJ.name = "MainPlayer";
        CacheManager.MP = PlayerOJ.GetComponent<MainPlayer>();
        //return PlayerOJ.GetComponent<MainPlayer>();    
        CacheManager.CacheInstantiatedPlayer(ClientID, PlayerOJ.GetComponent<MainPlayer>());        
    }

    // For developing purpose only
    private void LoadDevelopingPlayerData() {
        string FilePath = "TestData/testingplayer.txt";
        if (!File.Exists(FilePath)) {
            PlayerData = new PlayerData();
            PlayerData.Name = "Testing";
            string json = JsonUtility.ToJson(PlayerData);
            StreamWriter SaveStream = new StreamWriter(FilePath);
            SaveStream.Write(json);
            SaveStream.Close();
        } else {
            StreamReader LoadStream = new StreamReader(FilePath);
            string json = LoadStream.ReadToEnd();
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
        }
        CacheManager.CachePlayerData(Client.ClientID, PlayerData, ObjectIdentity.Main);
        CacheManager.CacheInstantiatedPlayer(Client.ClientID, this);
        CacheManager.MP = this;
    }
    private void SaveDevelopingPlayerData() {
        StreamWriter SaveStream = new StreamWriter("TestData/testingplayer.txt");
        string json = JsonUtility.ToJson(PlayerData);
        SaveStream.Write(json);
        SaveStream.Close();
    }
    
    protected override void Awake() {        
        base.Awake();
        
        //For developing purpose
        if (!Client.Connected) {
            LoadDevelopingPlayerData();
        }

        //Testing...
        GetComponent<DropList>().SpawnLoots();        
        //Testing...

        MPUI = GetComponentInChildren<MainPlayerUI>(true);
        //PlayerData = SaveLoadManager.LoadPlayerInfo(SaveLoadManager.SlotIndexToLoad);
        MainCamera = transform.GetComponentInChildren<Camera>();        
    }

    protected override void Start() {
        base.Start();
        TopNotification.Push("~ "+Scene.Current_Name+" ~", MyColor.White, 3f);
    }

    protected override void Update() {
        Client.Send(Protocols.UpdatePlayerPosition, new PositionData(Client.ClientID, Position));
        base.Update();        
        InteractionUpdate();        
    }

    protected override void Die() {
        base.Die();
        MainCamera.transform.parent = null;
        TopNotification.Push("Your soul is fading...",MyColor.Red,3f);
        Destroy(transform.gameObject,4f);        
        Scene.LoadWithDelay(ID.Developing, 5f);        
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();                
    }

    protected override void ControlUpdate() {
        if (Client.Connected) {
            NetworkControlUpdate();
        } else {
            SinglePlayerControlUpdate();
        }
    }

    //These two function will be deleted
    protected void NetworkControlUpdate() {
        if (Stunned || !Alive) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
        } else if (Casting) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
            Direction = ControllerManager.Direction;
        } else {
            if (GetWC() != null && GetCurrStats(STATSTYPE.ESSENSE) - GetWC().EssenseCost < 0) {
                AttackVector = Vector2.zero;
                if (ControllerManager.AttackVector != Vector2.zero)
                    RedNotification.Push(RedNotification.Type.NO_MANA);
            } else {
                if (AttackVector != ControllerManager.AttackVector)
                    Client.Send(Protocols.UpdatePlayerAttackVector, new AttackData(Client.ClientID, ControllerManager.AttackVector));
            }
            if (HasForce()) {
                MoveVector = Vector2.zero;
            } else {
                if (MoveVector != ControllerManager.MoveVector) {
                    Client.Send(Protocols.UpdatePlayerMoveVector, new MovementData(Client.ClientID, ControllerManager.MoveVector));                    
                }
            }
            if (Direction != ControllerManager.Direction) {
                Client.Send(Protocols.UpdatePlayerDirection, new DirectionData(Client.ClientID, ControllerManager.Direction));
            }
        }
    }
    protected void SinglePlayerControlUpdate() {//Single player
        if (Stunned || !Alive) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
        } else if (Casting) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
            Direction = ControllerManager.Direction;
        } else {
            if (GetWC() != null && GetCurrStats(STATSTYPE.ESSENSE) - GetWC().EssenseCost < 0) {
                AttackVector = Vector2.zero;
                if (ControllerManager.AttackVector != Vector2.zero)
                    RedNotification.Push(RedNotification.Type.NO_MANA);
            } else {
                AttackVector = ControllerManager.AttackVector;
            }
            if (HasForce()) {
                MoveVector = Vector2.zero;
            } else {
                MoveVector = ControllerManager.MoveVector;
            }
            Direction = ControllerManager.Direction;
        }
    }
    //These two function will be deleted

    void InteractionUpdate() {
        if (InteractTarget != null && ControllerManager.SyncActions) {
            InteractTarget.SetMessage();         
            InteractionNotification.TurnOn();
            if (ControllerManager.Actions.Submit.WasPressed) {
                InteractTarget.Interact();
            }
        } else
            InteractionNotification.TurnOff();
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer(CollisionLayer.Interaction)) {
            InteractTarget = collider.GetComponent<Interaction>();
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer(CollisionLayer.Interaction) && InteractTarget != null) {
            InteractTarget.Disengage();
        }
    }




    //Stats Handling
    public void AddStatPoint(string Stat) {
        //if (Stat == "Health")
        //    PlayerData.BaseHealth += StatModule.Health_Weight;
        //else if (Stat == "Mana")
        //    PlayerData.BaseMana += StatModule.Mana_Weight;
        //else if (Stat == "AD")
        //    PlayerData.BaseAD += StatModule.AD_Weight;
        //else if (Stat == "MD")
        //    PlayerData.BaseMD += StatModule.MD_Weight;
        //else if (Stat == "AttkSpd")
        //    PlayerData.BaseAttkSpd += StatModule.AttkSpd_Weight;
        //else if (Stat == "MoveSpd")
        //    PlayerData.BaseMoveSpd += StatModule.MoveSpd_Weight;
        //else if (Stat == "Defense")
        //    PlayerData.BaseDefense += StatModule.Defense_Weight;
        //else if (Stat == "CritChance")
        //    PlayerData.BaseCritChance += StatModule.CritChance_Weight;
        //else if (Stat == "CritDmgBounus")
        //    PlayerData.BaseCritDmgBounus += StatModule.CritDmgBounus_Weight;
        //else if (Stat == "LPH")
        //    PlayerData.BaseLPH += StatModule.LPH_Weight;
        //else if (Stat == "ManaRegen")
        //    PlayerData.BaseManaRegen += StatModule.ManaRegen_Weight;
        //PlayerData.StatPoints--;
        //SaveLoadManager.SaveCurrentPlayerInfo();
        //UpdateStats();
    }


    //Netorked related functions
    public override void Equip(Equipment E) {
        if (Client.Connected) {
            Client.Send(Protocols.EquipAction, new EquipActionData(Client.ClientID, E));
            base.Equip(E);
        } else {
            base.Equip(E);
            SaveDevelopingPlayerData();
        }

    }

    public override void UnEquip(EQUIPTYPE Slot) {
        if (Client.Connected) {
            Client.Send(Protocols.UnEquipAction, new UnEquipActionData(Client.ClientID, (int)Slot));
            base.UnEquip(Slot);
        } else {
            base.UnEquip(Slot);
            SaveDevelopingPlayerData();
        }
    }

    public override void AddToInventory(int Slot, Equipment E) {
        if (Client.Connected) {
            Client.Send(Protocols.AddtoInventoryAction, new AddToInventoryData(Slot, E));
            base.AddToInventory(Slot, E);
        } else {
            base.AddToInventory(Slot, E);
            SaveDevelopingPlayerData();
        }
    }

    public override void RemoveFromInventory(int Slot) {
        if (Client.Connected) {
            Client.Send(Protocols.RemoveFromInventoryAction, new RemoveFromInventoryData(Slot));
            base.RemoveFromInventory(Slot);
        } else {
            base.RemoveFromInventory(Slot);
            SaveDevelopingPlayerData();
        }
    }    
}
