using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSpawner : Spawner {

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public override GameObject Spawn() {
        GameObject temp = Instantiate(SpawnList[0]);
        temp.transform.position = transform.position;
        temp.name = temp.GetComponentInChildren<EnemyController>().GetName();
        return temp;
    }
}
