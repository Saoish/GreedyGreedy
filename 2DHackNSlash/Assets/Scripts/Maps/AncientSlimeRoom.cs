using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AncientSlimeRoom : MapController {
    public AudioClip Roar;
    private int WaveSlimesKilled;

    DropList LootSpawner;

    float SpawnTimer = 0f;
    [SerializeField]
    float SpawnInterval = 1f;

    float WaitTimer = 0f;
    float LastWaitTimer = 0f;
    [SerializeField]
    float WaitInterval = 30f;

    [SerializeField]
    int Wave = 1;
    int Spawned = 0;

    [SerializeField]
    public int BossSapwnWave = 10;

    MobSpawner[] Spawners;
    BossSpawner BossSpawner;


    bool AllowWait = false;
    bool AllowSpawn = false;
    bool BossFighting = false;

	// Use this for initialization
    protected override void Awake() {
        base.Awake();
    }

    protected override void Start () {
        base.Start();
        LootSpawner = GetComponentInChildren<DropList>();
        Spawners = GetComponentsInChildren<MobSpawner>();
        BossSpawner = GetComponentInChildren<BossSpawner>();
        BossSpawner.transform.GetComponent<SpriteRenderer>().sortingOrder = Layer.Ground;
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
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
        if(MPC==null)
            MPC = GameObject.Find("MainPlayer").GetComponent<MainPlayer>();
        if (MPC.PickedTarget==null && ControllerManager.AllowControlUpdate && (Input.GetKeyDown(ControllerManager.Interact) || Input.GetKeyDown(ControllerManager.J_A))) {
            TopNotification.Push("More slimes are coming!", MyColor.Orange);
            WaitTimer = 0;
            LastWaitTimer = 0;
            AllowWait = false;
            AllowSpawn = true;
            return;
        }
            

        if(WaitTimer == 0) {
            TopNotification.Push((WaitInterval).ToString("F0"), MyColor.Yellow);
        }
        if (WaitTimer - LastWaitTimer >= 1) {
            TopNotification.Push((WaitInterval-WaitTimer).ToString("F0"), MyColor.Yellow);
            LastWaitTimer = WaitTimer;          
        }
        if (WaitTimer < WaitInterval) {
            WaitTimer += Time.deltaTime;
        } else {
            TopNotification.Push("More slimes are coming!", MyColor.Orange);
            WaitTimer = 0;
            LastWaitTimer = 0;
            AllowWait = false;
            AllowSpawn = true;
        }
    }

    void ApplyOnDeathUpdate(GameObject EnemyOJ) {
        EnemyOJ.GetComponentInChildren<EnemyController>().LootDrop = false;
        EnemyOJ.GetComponentInChildren<EnemyController>().ON_DEATH_UPDATE += CondictionCheckOnDeath;
    }

    void CondictionCheckOnDeath() {
        WaveSlimesKilled++;
        if (WaveSlimesKilled >= (Wave-1) * Spawners.Length && !BossFighting) {
            if (Wave == BossSapwnWave && !BossFighting) {
                AllowWait = false;
                AllowSpawn = false;
                BossFighting = true;
                StartSummoningAncientSlime();
                HealPlayer();
                return;
            }
            AllowWait = true;
            LootSpawner.SpawnLoots();
            WaveSlimesKilled = 0;
            HealPlayer();
            //StartFilling();
        }
    }

    void HealPlayer() {
        if(MPC == null)
            MPC = GameObject.Find("MainPlayer").GetComponent<MainPlayer>();
        ModData HeallingBuffMod = ScriptableObject.CreateInstance<ModData>();
        HeallingBuffMod.Name = "HealingBuff";
        HeallingBuffMod.Duration = 5f;
        HeallingBuffMod.ModHealth = MPC.GetMaxHealth() * (5f / 100);
        GameObject HealingBuffObject = Instantiate(Resources.Load("BuffPrefabs/" + HeallingBuffMod.Name)) as GameObject;
        HealingBuffObject.name = "HealingBuff";
        HealingBuffObject.GetComponent<Buff>().ApplyBuff(HeallingBuffMod, MPC);
    }

    void StartFilling() {
        BossSpawner.transform.GetComponent<Animator>().SetTrigger("Fill");
    }

    public void Fill() {

    }

    void StartSummoningAncientSlime() {
        TopNotification.Push("...Something is wrong, what is the device at center?", MyColor.Red);
        Animator AncientBossSpawnerAnim = BossSpawner.transform.GetComponent<Animator>();
        AncientBossSpawnerAnim.SetTrigger("Spawn");
        AudioSource.PlayClipAtPoint(Roar, transform.position, GameManager.SFX_Volume);
    }

    public void SpawnAncientSlime() {
        BossSpawner.Spawn();
    }

    public void StartWait() {
        AllowWait = true;
    }
}
