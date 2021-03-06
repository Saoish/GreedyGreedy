﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSpawner : Spawner {
    public AudioClip test;
    // Use this for initialization
    void Start() {
        //AudioSource.PlayClipAtPoint(test, transform.position, GameManager.SFX_Volume);
        //GetComponent<AudioSource>().Play();
        //AudioSource.PlayClipAtPoint(test, transform.position, GameManager.SFX_Volume);
        //GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update() {
    }

    public override GameObject Spawn() {
        GameObject temp = Instantiate(SpawnList[0]);
        temp.transform.position = transform.position;
        temp.name = temp.GetComponentInChildren<Monster>().GetName();
        return temp;
    }
}
