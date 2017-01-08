using UnityEngine;
using System.Collections;
using System;

public class FriendlyPlayer : Player {
    //Not yet implemented
    //Not yet implemented
    //Not yet implemented
    //Not yet implemented
    //Not yet implemented
    //Not yet implemented

    public static void Instantiate(PlayerData PlayerData, Vector2 Position) {
        GameObject FriendlyPlayerOJ = Resources.Load("PlayerPrefabs/FriendlyPlayer") as GameObject;
        FriendlyPlayerOJ.GetComponent<MainPlayer>().PlayerData = PlayerData;
        FriendlyPlayerOJ = Instantiate(FriendlyPlayerOJ, Position, Quaternion.identity) as GameObject;
        FriendlyPlayerOJ.name = "FriendlyPlayer";
    }

    // Use this for initialization
    protected override void Awake() {
        base.Awake();
    }
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
    }

    protected override void ControlUpdate() {
        throw new NotImplementedException();
    }

    protected override void Die() {
        base.Die();
    }
}
