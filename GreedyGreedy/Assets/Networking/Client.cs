using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Networking;
using GreedyNameSpace;
using GreedyScene;
using Networking.Data;

namespace Networking {
    public class Client : MonoBehaviour {
        public bool RunningWithServerIP = false;

        private static Package Package;                        
        public static bool Connected;        

        public static int HostID = 0;//Always 0, server's identity
        public static int ClientID = -1; //Fetch from server, for network identity, -1 is not identified

        private static int connectionID;//Always be 1, and for local identity

        static int TCP, UDP;
        static int maxConnections = 1;//Always be 1 for client

        static int clientSocket;
        //EC2 Static address
        //string ip = "52.36.226.238";
        static string ip = String.Empty;
        static int port = 27550;

        public static Client instance;

        //---The following part for LAN hole punching
        int BroadcastKey = 999;
        int BroadcastVersion = 1000;
        int BroadcastSubVersion = 1001;
        string BroadcastCipher = "#FASF#F@S@#RSR!REWGW_@WF!W";
        void SetupLANCredentials() {
            byte error;
            NetworkTransport.SetBroadcastCredentials(clientSocket, BroadcastKey, BroadcastVersion, BroadcastSubVersion, out error);
        }
        //void StartBroadCasting() {
        //    byte error;
        //    byte[] scream_buffer = Serializer.Serialize(BroadcastCipher);
        //    NetworkTransport.StartBroadcastDiscovery(clientSocket, port, BroadcastKey, BroadcastVersion, BroadcastSubVersion, scream_buffer, scream_buffer.Length, 1000, out error);
        //}
        //void StopBroadCasting() {
        //    NetworkTransport.StopBroadcastDiscovery();
        //}
        //---The following part for LAN hole punching      

        void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            } else {
                instance = this;
                DontDestroyOnLoad(this);
            }
            DataManager.LoadUserName();            
        }

        // Use this for initialization
        void Start() {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            TCP = config.AddChannel(QosType.Reliable);
            UDP = config.AddChannel(QosType.Unreliable);            
            HostTopology topology = new HostTopology(config, maxConnections);            
            if (RunningWithServerIP) {                
                clientSocket = NetworkTransport.AddHost(topology);
                ip = Network.player.ipAddress;
            } else {
                clientSocket = NetworkTransport.AddHost(topology,port);
            }           
            SetupLANCredentials();            
        }       

        // Update is called once per frame
        void Update() {
            Process();
        }

        public static void ShutDown() {
            NetworkTransport.Shutdown();            
        }

        public static void Connect() {
            if (Connected || ip == string.Empty)
                return;
            byte error;
            Debug.Log(ip);
            connectionID = NetworkTransport.Connect(clientSocket, ip, port,0, out error);            
        }

        static void _Send(byte[] binary_data, int channel = 0) {//Default by TCP channel
            byte error;
            if (binary_data.Length > 1024) {
                Debug.Log("Not sent, this package contain size of MTU.");
            }
            NetworkTransport.Send(clientSocket, connectionID, channel, binary_data, binary_data.Length, out error);
        }

        void Process() {
            //int HostID = 0;
            //int ClientID;

            int Channel;
            int DataSize;
            byte[] buffer = new byte[1024];
            byte error;

            NetworkEventType networkEvent = NetworkEventType.DataEvent;
            do {                
                networkEvent = NetworkTransport.ReceiveFromHost(HostID, out connectionID, out Channel, buffer, 1024, out DataSize, out error);                
                switch (networkEvent) {
                    case NetworkEventType.ConnectEvent:// Server received connect event    
                        Connected = true;
                        Debug.Log("Connected.");
                        break;
                    case NetworkEventType.DataEvent:// Server received data                            
                        try {//Try catch is super expensive, a data server will replace it later
                            Decipher d = Serializer.UnSeal(HostID, buffer);
                            SendMessage(d.protocol, d);
                        } catch {
                            AppendBuffer(buffer, DataSize);
                        }      
                        break;
                    case NetworkEventType.DisconnectEvent:// Client received disconnect event
                        Connected = false;
                        if(error == 6) {//Server down                            
                            StartCoroutine(Scene.LoadThenExecute(ID.StartMenu, PopUpNotification.Push, "Disconnected from server.",PopUpNotification.Type.Confirm));
                        }                        
                        Debug.Log("Disconnected.");
                        ClientID = -1;
                        break;
                    case NetworkEventType.BroadcastEvent://This block should be removed for network version                        
                        if (ip != String.Empty)
                            return;                        
                        NetworkTransport.GetBroadcastConnectionMessage(HostID, buffer, 1024, out DataSize, out error);                        
                        ip = Serializer.DeSerialize<string>(buffer);
                        break;
                }
            } while (networkEvent != NetworkEventType.Nothing);
        }
        
        public static void Send(Protocols Protocol) {
            byte[] data = Serializer.Seal(Protocol);
            _Send(data, 0);
        }

        public static void Send<T>(Protocols Protocol, T instance, int Channel = 0){
            byte[] data = Serializer.Seal(Protocol, instance);
            _Send(data, Channel);
        }

        public static void ManuallyDisconnectSelf() {
            byte error;
            NetworkTransport.Disconnect(HostID, connectionID, out error);
            ClientID = -1;
        }
        







        //Client Protocols
        private void Identify(Decipher decipher) {//Any instance based action required Identify self first, and this will be done by server once login
            ClientID = int.Parse(decipher.content);
        }

        private void CreateUsername(Decipher decipher) {
            DataManager.CreateUsernameProfile(decipher.content);            
            PopUpNotification.Push("Successfully registered.", PopUpNotification.Type.Confirm);
        }

        private void PopUpNotify(Decipher decipher) {
            PopUpNotification.Push(decipher.content, PopUpNotification.Type.Confirm);
        }        

        private void TopNotify(Decipher decipher) {            
            TopNotifyData temp = JsonUtility.FromJson<TopNotifyData>(decipher.content);
            TopNotification.Push(temp.message,temp.color,temp.period);
        }

        private void UpdateLoadingText(Decipher decipher) {
            LoadingUI.LoadingText = decipher.content;
        }

        private void DisconnectByServer(Decipher decipher) {
            byte error;
            NetworkTransport.Disconnect(decipher.tail, ClientID , out error);            
            if (decipher.content != string.Empty)
                PopUpNotification.Push(decipher.content, PopUpNotification.Type.Confirm);     
        }

        private void ListeningForPacakage(Decipher decipher) {           
            Package = JsonUtility.FromJson<Package>(decipher.content);
            Package.Initialize();            
        }        

        private void LoadVillage(Decipher decipher) {            
            CacheManager.CachePlayerData(ClientID, DataManager.UserData.PlayerDatas[CacheManager.CachedPlayerSlotIndex], ObjectIdentity.Main);//Cache Main Player Data
            Scene.LoadWithAction(ID.Village, CacheManager.InstantiatePlayers);     
        }

        private void LoadSceneWithSync(Decipher decipher) {
            Scene.LoadWithManualActive(JsonUtility.FromJson<ID>(decipher.content));            
        }

        private void SyncLoadedScene(Decipher decipher) {
            Scene.ActiveScene(1f, CacheManager.InstantiatePlayers);
        }

        //Flag
        private void EquipAction(Decipher decipher) {
            EquipActionData temp = JsonUtility.FromJson<EquipActionData>(decipher.content);
            if (CacheManager.Players[temp.ClientID].PC)                
                CacheManager.Players[temp.ClientID].PC.Equip(temp.E);
        }

        private void UnEquipAction(Decipher decipher) {
            UnEquipActionData temp = JsonUtility.FromJson<UnEquipActionData>(decipher.content);
            if (CacheManager.Players[temp.ClientID].PC)               
                CacheManager.Players[temp.ClientID].PC.UnEquip((EQUIPTYPE)temp.Slot);
        }

        private void UpdatePlayerMoveVector(Decipher decipher) {
            MovementData temp = JsonUtility.FromJson<MovementData>(decipher.content);
            if (CacheManager.Players[temp.ClientID].PC) {
                CacheManager.Players[temp.ClientID].PC.MoveVector = temp.MoveVector;                
            }
        }

        private void UpdatePlayerAttackVector(Decipher decipher) {
            AttackData temp = JsonUtility.FromJson<AttackData>(decipher.content);
            if (CacheManager.Players[temp.ClientID].PC)                
                CacheManager.Players[temp.ClientID].PC.AttackVector = temp.AttackVector;
        }

        private void UpdatePlayerDirection(Decipher decipher) {
            DirectionData temp = JsonUtility.FromJson<DirectionData>(decipher.content);            
            if (CacheManager.Players[temp.ClientID].PC)                
                CacheManager.Players[temp.ClientID].PC.Direction = temp.Direction;
        }

        private void UpdatePlayerPosition(Decipher decipher) {
            PositionData temp = JsonUtility.FromJson<PositionData>(decipher.content);
            if (CacheManager.Players[temp.ClientID].PC) {
                if(Mathf.Abs(Vector2.Distance(temp.Position,CacheManager.Players[temp.ClientID].PC.Position))>0.2f)
                    CacheManager.Players[temp.ClientID].PC.Position = temp.Position;
            }
        }

        private void PopQueueEntrance(Decipher decipher) {            
            Interaction.TurnOffAllInteractionWindow();
            ControllerManager.SyncActions = false;
            PopUpNotification.Push("Your queue is ready, do you want to join?",PopUpNotification.Type.Select);
            StartCoroutine(PopUpNotification.WaitForDecisionThenPerformAction(AcceptEntrance, DenyEntrance));
        }


        //Helper
        private void AcceptEntrance() {
            Send(Protocols.AcceptMatchMakingEntrance);
            ControllerManager.SyncActions = true;            
        }

        private void DenyEntrance() {
            Send(Protocols.DenyMatchMakingEntrance);
            ControllerManager.SyncActions = true;
        }




        //Pacakage control
        private void AppendBuffer(byte[] buffer, int dataSize) {
            System.Buffer.BlockCopy(buffer, 0, Package.buffer, Package.size, dataSize);
            Package.size += dataSize;
            if (Package.size == Package.length)
                gameObject.SendMessage(Package.protocol.ToString());
        }

        private void LoadUserData() {            
            DataManager.LoadUserData(Serializer.DeSerialize<UserData>(Package.buffer));
            Scene.Load(ID.CharacterSelection);            
        }

        private void LoadUpdatedPlayerData() {
            DataManager.LoadPlayerData(Serializer.DeSerialize<PlayerData>(Package.buffer));
            Scene.Load(ID.CharacterSelection);
        }

        private void LoadPlayerIdentities() {
            List<PlayerIdentity> PIDs = Serializer.DeSerialize<List<PlayerIdentity>>(Package.buffer);                
            foreach(var PID in PIDs) {
                //Debug.Log("MyClientID: " + ClientID + ", EnemyClietnID: " + PID.ClientID + " (" + PID.PlayerData.Name + ")");
                CacheManager.CachePlayerData(PID.ClientID, PID.PlayerData, PID.ID);
            }            
            Send(Protocols.ReadyUp);
        }
    }
}

