using UnityEngine;
using System.Collections;

public class SaveLoadManager : MonoBehaviour {
    public static int SlotIndexToLoad;

    public static SaveLoadManager instance;
    public static SaveLoadManager Instance { get { return instance; } }

    void Awake() {
        DataManager.Load();
        if (instance == null) {
            DontDestroyOnLoad(this);
            instance = this;
        }else {
            Destroy(gameObject);
        }
    }

    public static void SaveCurrentPlayerInfo() {
        PlayerController PC = GameObject.Find("MainPlayer/PlayerController").GetComponent<PlayerController>();
        DataManager.SaveCharacter(PC.GetPlayerData());
        DataManager.Save();
    }

    public static CharacterDataStruct LoadPlayerInfo(int SlotIndex) {
        return DataManager.LoadCharacter(SlotIndex);
    }
}
