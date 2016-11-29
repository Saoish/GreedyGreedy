using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AncientSlimeRoom : LevelManager {
    public int NumberOfSlimesToKilled = 10;

    [SerializeField]
    private int WaveSlimesKilled;

    [SerializeField]
    private int CachedSlimesKilled;

    DropList LootSpawner;

    float SpawnTimer = 0f;
    float SpawnInterval = 2f;

    float GapTimer = 0f;
    float LastGapTimer = 0f;
    float GapInterval = 30f;

    int Wave = 1;
    int Spawned = 0;

    MobSpawner[] Spawners;
    BossSpawner BossSpawner;

    bool AllowSpawn = false;

    bool AllowWait = true;

    bool BossFighting = false;

	// Use this for initialization
	void Start () {
        LootSpawner = GetComponentInChildren<DropList>();
        Spawners = GetComponentsInChildren<MobSpawner>();
        BossSpawner = GetComponentInChildren<BossSpawner>();
    }
	
	// Update is called once per frame
	void Update () {
        if (AllowWait)
            Waiting();
        else if(AllowSpawn)
            Spawining();
    }

    void Spawining() {
        if (SpawnTimer < SpawnInterval) {
            SpawnTimer += Time.deltaTime;
        } else {
            foreach (Spawner s in Spawners) {
                Spawned++;
                if (Spawned >= Wave * Spawners.Length) {
                    AllowSpawn = false;
                    Wave++;
                    Spawned = 0;
                }
                ApplyOnDeathUpdate(s.Spawn());
            }
            SpawnTimer = 0;
        }
    }

    void Waiting() {
        if (GapTimer - LastGapTimer >= 1) {
            LastGapTimer = GapTimer;
        }
        if (GapTimer < GapInterval) {
            GapTimer += Time.deltaTime;
        } else {
            GapTimer = 0;
            AllowWait = false;
            AllowSpawn = true;
        }
    }

    void ApplyOnDeathUpdate(GameObject EnemyOJ) {
        EnemyOJ.GetComponentInChildren<EnemyController>().LootDrop = false;
        EnemyOJ.GetComponentInChildren<EnemyController>().ON_DEATH_UPDATE += CondictionCheck;
    }

    void CondictionCheck() {
        WaveSlimesKilled++;
        CachedSlimesKilled++;
        if (CachedSlimesKilled == NumberOfSlimesToKilled) {
            AllowWait = false;
            AllowSpawn = false;
            BossFighting = true;
            BossSpawner.Spawn();
        }
        else if(WaveSlimesKilled >= (Wave-1) * Spawners.Length && !BossFighting) {
            AllowWait = true;
            LootSpawner.SpawnLoots();
            WaveSlimesKilled = 0;
            HealPlayer();
        }
    }

    void HealPlayer() {
        MainPlayer MPC = GameObject.Find("MainPlayer/PlayerController").GetComponent<MainPlayer>();
        ModData HeallingBuffMod = ScriptableObject.CreateInstance<ModData>();
        HeallingBuffMod.Name = "HealingBuff";
        HeallingBuffMod.Duration = 5f;
        HeallingBuffMod.ModHealth = MPC.GetMaxHealth() * (5f / 100);
        GameObject HealingBuffObject = Instantiate(Resources.Load("BuffPrefabs/" + HeallingBuffMod.Name)) as GameObject;
        HealingBuffObject.name = "HealingBuff";
        HealingBuffObject.GetComponent<Buff>().ApplyBuff(HeallingBuffMod, MPC);
    }
}
