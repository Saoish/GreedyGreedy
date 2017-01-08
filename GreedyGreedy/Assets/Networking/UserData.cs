using UnityEngine;
using System.Collections;
using GreedyNameSpace;

[System.Serializable]
public class UserData {
    public PlayerData[] PlayerDatas;

    public UserData() {
        PlayerDatas = new PlayerData[Patch.CharacterSlots];
    }
}
