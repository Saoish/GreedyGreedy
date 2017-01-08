using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using GreedyNameSpace;
using GreedyScene;


public static class DataManager {
    public static string Raw_UserData;    
    public static string Username;
    public static UserData UserData = new UserData();
    

    public static void LoadUserData(UserData userdata) {
        UserData = userdata;        
    }

    public static void LoadPlayerData(PlayerData PlayerData) {
        UserData.PlayerDatas[PlayerData.SlotIndex] = PlayerData;        
    }

    public static void LoadUserName() {
        if (new DirectoryInfo("Data").GetFiles().Length <= 0)
            return;
        else {
            StreamReader LoadStream = new StreamReader("Data/username.txt");
            Username = LoadStream.ReadToEnd();
            LoadStream.Close();
        }
    }

    public static void CreateUsernameProfile(string username) {
        StreamWriter SaveStream = new StreamWriter("Data/username.txt");
        SaveStream.Write(username);
        SaveStream.Close();
        LoadUserName();
    }    

    public static PlayerData GetPlayerData(int SlotIndex) {
        return UserData.PlayerDatas[SlotIndex];
    }
    

}
