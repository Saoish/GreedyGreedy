using UnityEngine;
using System.Collections;
using System;

public class EnemyPlayer : Player {    
    public static void Instantiate(int ClientID,PlayerData PlayerData, Vector2 Position) {
        GameObject PlayerOJ = Resources.Load("PlayerPrefabs/EnemyPlayer") as GameObject;
        PlayerOJ.GetComponent<EnemyPlayer>().PlayerData = PlayerData;
        PlayerOJ = Instantiate(PlayerOJ, Position, Quaternion.identity) as GameObject;
        PlayerOJ.name = "EnemyPlayer";
        PlayerOJ.GetComponent<EnemyPlayer>().Lerps = new LerpQueue(PlayerOJ.GetComponent<EnemyPlayer>());
        CacheManager.CacheInstantiatedPlayer(ClientID, PlayerOJ.GetComponent<EnemyPlayer>());
    }

    // Use this for initialization
    protected override void Awake() {
        base.Awake();
    }
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void ControlUpdate() {
        //
    }

    protected override void Die() {
        base.Die();
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == Tag.FriendlyPlayer) {
            collision.collider.transform.parent.GetComponent<ObjectController>().MountainlizeRigibody();
        }
    }
    //void OnCollusionExite2D(Collision2D collision) {

    //}
}
