using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Networking.Data;
using GreedyNameSpace;

public class PlayerPointer {    
    public PlayerData PlayerData;
    public ObjectIdentity ID;
    public Player PC = null;
    public PlayerPointer(PlayerData PlayerData, ObjectIdentity ID) {
        this.PlayerData = PlayerData;
        this.ID = ID;
    }
}

public class CacheManager : MonoBehaviour {//This class use to cach every during scene transition
    public static int CachedPlayerSlotIndex;

    public static MainPlayer MP;
    public static Dictionary<int, PlayerPointer> Players = new Dictionary<int, PlayerPointer>();    

    public static CacheManager instance;    

    void Awake() {        
        if (instance == null) {
            DontDestroyOnLoad(this);
            instance = this;
        }else {
            Destroy(gameObject);
        }
    }


    public static void CachePlayerData(int ClientID,PlayerData PlayerData, ObjectIdentity ID) {
        Players[ClientID] = new PlayerPointer(PlayerData, ID);
    }    


    public static void CacheInstantiatedPlayer(int ClientID, Player PC) {
        Players[ClientID].PC = PC;
        Players[ClientID].PlayerData = PC.PlayerData;
        
    }

    public static PlayerData GetPlayerData(int ClientID) {
        return Players[ClientID].PlayerData;
    }

    public static void InstantiatePlayers() {
        foreach(var p in Players) {
            switch (p.Value.ID) {
                case ObjectIdentity.Main:
                    MainPlayer.Instantiate(p.Key,p.Value.PlayerData, Vector2.zero);
                    break;
                case ObjectIdentity.Enemy:
                    EnemyPlayer.Instantiate(p.Key,p.Value.PlayerData, Vector2.zero);
                    break;
            }            
        }
    }
}
