using UnityEngine;
using System.Collections;
using GreedyNameSpace;
using Networking;

namespace Networking.Data {

    public enum Protocols {
        //Server Protocols
        RegisterUser,
        UserLogin,
        CreateCharacter,
        DeleteCharacter,
        SubscribeIdentityAndInstance,

        AddtoInventoryAction,
        RemoveFromInventoryAction,

        QueUpForArena_1v1,
        AcceptMatchMakingEntrance,
        DenyMatchMakingEntrance,
        ReadyUp,

        //Client Protocols
        Identify,
        CreateUsername,
        PopUpNotify,
        TopNotify,
        UpdateLoadingText,
        DisconnectByServer,
        AppendBuffer,
        ListeningForPacakage,
        LoadUserData,
        LoadUpdatedPlayerData,
        LoadVillage,
        LoadSceneWithSync,
        SyncLoadedScene,
        PopQueueEntrance,
        LoadPlayerIdentities,        


        /*
        Duplicated protocols
        */
        EquipAction,
        UnEquipAction,
        UpdatePlayerMoveVector,
        UpdatePlayerAttackVector,
        UpdatePlayerDirection,
        UpdatePlayerPosition
    }

    public enum Side {
        Blue,
        Red
    }

    [System.Serializable]
    public class PlayerIdentity {
        public int ClientID;
        public ObjectIdentity ID;
        public PlayerData PlayerData;
        public PlayerIdentity(int ClientID, ObjectIdentity ID, PlayerData PlayerData) {
            this.ClientID = ClientID;
            this.ID = ID;
            this.PlayerData = PlayerData;
        }
    }

    [System.Serializable]
    public class TopNotifyData {
        public string message;
        public Color color;
        public float period;
        public TopNotifyData(string message, Color color, float period) {
            this.message = message;
            this.color = color;
            this.period = period;
        }
    }

    [System.Serializable]
    public class PositionData {
        public int ClientID;
        public Vector2 Position;
        public PositionData(int ClientID, Vector2 Position) {
            this.ClientID = ClientID;
            this.Position = Position;          
        }
    }

    [System.Serializable]
    public class DirectionData {
        public int ClientID;
        public int Direction;
        public DirectionData(int ClientID, int Direction) {
            this.ClientID = ClientID;
            this.Direction = Direction;
        }
    }

    [System.Serializable]
    public class AttackData {
        public int ClientID;
        public Vector2 AttackVector;
        public AttackData(int ClientID, Vector2 AttackVector) {
            this.ClientID = ClientID;
            this.AttackVector = AttackVector;
        }
    }

    [System.Serializable]
    public class MovementData {
        public int ClientID;
        public Vector2 MoveVector;        
        public MovementData(int ClientID, Vector2 MoveVector) {
            this.ClientID = ClientID;
            this.MoveVector = MoveVector;            
        }
    }

    [System.Serializable]
    public class UnEquipActionData {
        public int ClientID;
        public int Slot;
        public UnEquipActionData(int ClientID, int Slot) {
            this.ClientID = ClientID;
            this.Slot = Slot;
        }
    }

    [System.Serializable]
    public class EquipActionData {
        public int ClientID;        
        public Equipment E;
        public EquipActionData(int ClientID, Equipment E) {
            this.ClientID = ClientID;
            this.E = E;
        }
    }


    [System.Serializable]
    public class RemoveFromInventoryData {
        public int Slot;
        public RemoveFromInventoryData(int Slot) {
            this.Slot = Slot;
        }
    }

    [System.Serializable]
    public class AddToInventoryData {
        public int Slot;
        public Equipment E;
        public AddToInventoryData(int Slot, Equipment E) {
            this.Slot = Slot;
            this.E = E;
        }
    }

    [System.Serializable]
    public class CreationData {
        public int SlotIndex;
        public RGB SkinColor;
        public string Name;
        public CLASS Class;
        public CreationData(int SlotIndex, RGB SkinColor, string Name, CLASS Class) {
            this.SlotIndex = SlotIndex;
            this.SkinColor = SkinColor;
            this.Name = Name;
            this.Class = Class;
        }
    }

    [System.Serializable]
    public class DeletionData {
        public int SlotIndex;
        public DeletionData(int SlotIndex) {
            this.SlotIndex = SlotIndex;
        }
    }

    [System.Serializable]
    public class RGB {
        public float R;
        public float G;
        public float B;
        public RGB() { R = G = B = 1; }
        public RGB(float R, float G, float B) {
            this.R = R;
            this.G = G;
            this.B = B;
        }
    }


    [System.Serializable]
    public struct Decipher {
        public string protocol;
        public string content;
        public int tail;

        public Decipher(string protocol, string content, int tail) {
            this.protocol = protocol;
            this.content = content;
            this.tail = tail;
        }
    }

    [System.Serializable]
    public class Package {
        public byte[] buffer;
        public int size;
        public int length;
        public Protocols protocol;
        public Package(int length, Protocols protocol) {
            this.length = length;
            this.protocol = protocol;
            size = 0;
        }
        public void Initialize() {
            buffer = new byte[length];
        }
    }
}
