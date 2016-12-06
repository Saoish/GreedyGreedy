using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GreedyNameSpace;

public abstract class PlayerController : ObjectController {
    protected int NextLevelExp = -999;

    public AudioClip hurt;
    public AudioClip crit_hurt;
    public AudioClip lvlup;

    protected CharacterDataStruct PlayerData;

    protected Dictionary<EquipType, GameObject> EquipPrefabs;

    protected GameObject BaseModel;

    protected WeaponController WC;

    protected string Name;

    [HideInInspector]
    public GameObject SkillTree;

    protected override void Awake() {
        base.Awake();
        EquipPrefabs = new Dictionary<EquipType, GameObject>();
    }

    protected override void Start() {
        base.Start();
        InitPlayer();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        ControlUpdate();        
        EquiPrefabsUpdate();
        BaseModelUpdate();
        Regen();
    }

    protected abstract void ControlUpdate();


    //----------public

    public WeaponController GetWC() {
        return WC;
    }

    //EXP handling
    public void AddEXP(int exp) {
        if (PlayerData.lvl < LvlExpModule.LvlCap) {
            PlayerData.exp += exp;
            CheckLevelUp();
        }
        SaveLoadManager.SaveCurrentPlayerInfo();
    }

    //Combat
    override public void DeductHealth(Value dmg) {
        if (dmg.Pop_Update)
            IC.PopUpText(dmg);
        if (dmg.IsCrit) {
            Animator Anim = VisualHolder.GetComponent<Animator>();
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
            if (dmg.SFX_Update)
                AudioSource.PlayClipAtPoint(crit_hurt, transform.position, GameManager.SFX_Volume);
        } else {
            if (dmg.SFX_Update)
                AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        }
        if (CurrStats.Get(StatsType.HEALTH) - dmg.Amount <= 0 && Alive) {
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
        } else {
            CurrStats.Dec(StatsType.HEALTH, dmg.Amount);
        }
    }

    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical("Body Parts Explode");

    }

    //-------private
    protected void ReloadWeaponModel() {
        if (PlayerData.Equipments[EquipType.Weapon] != null) {
            Destroy(EquipPrefabs[EquipType.Weapon]);
            EquipPrefabs[EquipType.Weapon] = EquipmentController.ObtainPrefab(PlayerData.Equipments[EquipType.Weapon], transform);
            FetchWC();
        }
    }

    private void FetchWC() {
        if (PlayerData.Equipments[EquipType.Weapon] == null) {
            WC = null;
            return;
        }
        WC = EquipPrefabs[EquipType.Weapon].GetComponent<WeaponController>();
    }

    void InitPlayer() {
        InstaniateEquipmentModel();

        if (PlayerData.lvl < LvlExpModule.LvlCap)
            NextLevelExp = LvlExpModule.GetRequiredExp(PlayerData.lvl + 1);

        FetchWC();

        InitMaxStats();
        InitSkillTree();
        InitOnCallEvent();
        InitPassives();
        InitCurrStats();
    }

    protected void InitPassives() {
        //Debug.Log(Passives.GetComponentsInChildren<PassiveSkill>().Length);
        foreach (var passive in T_Passives.GetComponentsInChildren<PassiveSkill>()) {
            passive.ApplyPassive();
        }
    }

    protected Transform GetActiveSkill_T(System.Type active_skill_type) {
        foreach(Transform skill_t in T_Actives) {
            if (skill_t.GetComponent<Skill>().GetType() == active_skill_type)
                return skill_t;
        }
        return null;
    }

    protected Transform GetPassiveSkill_T(System.Type passive_skill_type) {
        foreach (Transform skill_t in T_Passives) {
            //Debug.Log(skill_t.GetComponent<Skill>().GetType());
            if (skill_t.GetComponent<Skill>().GetType() == passive_skill_type)
                return skill_t;
        }
        return null;
    }

    protected void InitSkillTree() {
        if (PlayerData.Class == Class.Warrior) {
            SkillTree = Instantiate(Resources.Load("SkillPrefabs/WarriorSkillTree"), transform) as GameObject;
            SkillTree.name = "SkillTree";
        }
        else if(PlayerData.Class == Class.Mage) {

        }else if(PlayerData.Class == Class.Rogue) {

        }
        SkillTreeController STC = SkillTree.GetComponent<SkillTreeController>();
        for (int i = 0; i < PlayerData.SkillTreelvls.Length; i++) {
            if (STC.SkillTree[i] != null && PlayerData.SkillTreelvls[i] != 0) {//Does lvl+skill check here
                GameObject Skill_OJ = Instantiate(Resources.Load("SkillPrefabs/" + STC.SkillTree[i].Name)) as GameObject;
                Skill_OJ.transform.GetComponent<Skill>().InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
            }
        }
    }

    protected void InitMaxStats() {
        Name = PlayerData.Name;
        for(int i = 0; i < Stats.Size; i++) {
            MaxStats.Set(i, PlayerData.BaseStats.Get(i));
        }
        foreach (var e in PlayerData.Equipments) {
            if (e.Value != null) {
                for (int i = 0; i < Stats.Size; i++) {
                    MaxStats.Add(i, e.Value.Stats.Get(i));
                }
            }           
        }
    }

    protected void InitCurrStats() {
        for (int i = 0; i < Stats.Size; i++) {
            CurrStats.Set(i, MaxStats.Get(i));
        }
    }

    protected void BaseModelUpdate() {
        Animator BaseModelAnim = BaseModel.GetComponent<Animator>();
        BaseModelAnim.SetInteger("Direction", Direction);
        BaseModelAnim.speed = GetMovementAnimSpeed();
    }

    protected void EquiPrefabsUpdate() {
        foreach(var e_prefab in EquipPrefabs.Values) {
            if(e_prefab!=null)
                e_prefab.GetComponent<EquipmentController>().EquipUpdate(GetComponent<PlayerController>());
        }    
    }

    //-------helper
    protected void UpdateStats() {
        InitMaxStats();
        ReloadWeaponModel();
        InitOnCallEvent();
        InitPassives();

        for (int i = 0; i < Stats.Size; i++) {
            if ((int)StatsType.HEALTH == i) {
                if (CurrStats.Get(i) > MaxStats.Get(i))
                    CurrStats.Set(i,MaxStats.Get(i));
            }
            else if ((int)StatsType.MANA == i) {
                if (CurrStats.Get(i) > MaxStats.Get(i))
                    CurrStats.Set(i, MaxStats.Get(i));
            } else {
                CurrStats.Set(i, MaxStats.Get(i));
            }
        }
    }

    protected void UpdateSkillsState() {
        SkillTreeController STC = SkillTree.GetComponent<SkillTreeController>();
        for (int i = 0; i < PlayerData.SkillTreelvls.Length; i++) {
            if (STC.SkillTree[i].GetType().IsSubclassOf(typeof(ActiveSkill))) {
                if (GetActiveSkill_T(STC.SkillTree[i].GetType()) && PlayerData.SkillTreelvls[i] != 0)
                    GetActiveSkill_T(STC.SkillTree[i].GetType()).GetComponent<Skill>().InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                else if (GetActiveSkill_T(STC.SkillTree[i].GetType()) && PlayerData.SkillTreelvls[i] == 0)
                    Destroy(GetActiveSkill_T(STC.SkillTree[i].GetType()).gameObject);
                else if (PlayerData.SkillTreelvls[i] != 0 && !GetActiveSkill_T(STC.SkillTree[i].GetType())) {
                    GameObject Skill_OJ = Instantiate(Resources.Load("SkillPrefabs/" + STC.SkillTree[i].Name)) as GameObject;
                    Skill_OJ.transform.GetComponent<Skill>().InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                }
            } else {
                if (GetPassiveSkill_T(STC.SkillTree[i].GetType()) && PlayerData.SkillTreelvls[i] != 0)
                    GetPassiveSkill_T(STC.SkillTree[i].GetType()).GetComponent<Skill>().InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                else if (GetPassiveSkill_T(STC.SkillTree[i].GetType()) && PlayerData.SkillTreelvls[i] == 0)
                    Destroy(GetPassiveSkill_T(STC.SkillTree[i].GetType()).gameObject);
                else if (PlayerData.SkillTreelvls[i] != 0 && !GetActiveSkill_T(STC.SkillTree[i].GetType())) {
                    GameObject Skill_OJ = Instantiate(Resources.Load("SkillPrefabs/" + STC.SkillTree[i].Name)) as GameObject;
                    Skill_OJ.transform.GetComponent<Skill>().InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                }
            }
        }
    }

    protected void InitOnCallEvent() {
        ON_DMG_DEAL = null;
        ON_HEALTH_UPDATE = null;
        ON_MANA_UPDATE = null;
        ON_DEATH_UPDATE = null;
    }

    protected void InstaniateEquipmentModel() {
        if (PlayerData.Class == Class.Warrior) {
            BaseModel = Instantiate(Resources.Load("BaseModelPrefabs/Red Ghost"), VisualHolder) as GameObject;
            BaseModel.name = "Red Ghost";
            BaseModel.transform.position = VisualHolder.position + BaseModel.transform.position;
            BaseModel.transform.GetComponent<SpriteRenderer>().sortingLayerName = "Object";
        }
        else if(PlayerData.Class == Class.Mage) {

        }
        else if(PlayerData.Class == Class.Rogue) {

        }
        foreach(var e in PlayerData.Equipments) {
            if (e.Value != null) {
                GameObject equipPrefab = EquipmentController.ObtainPrefab(e.Value, transform);
                EquipPrefabs[e.Key] = equipPrefab;
            }
        }
    }

    protected void CheckLevelUp() {
        if (PlayerData.lvl >= LvlExpModule.LvlCap)
            return;
        if(PlayerData.exp >= NextLevelExp) {
            PlayerData.lvl++;
            PlayerData.exp = 0;
            CurrStats.Set(StatsType.HEALTH,MaxStats.Get(StatsType.HEALTH));
            CurrStats.Set(StatsType.MANA, MaxStats.Get(StatsType.MANA));
            NextLevelExp = LvlExpModule.GetRequiredExp(PlayerData.lvl + 1);
            AudioSource.PlayClipAtPoint(lvlup, transform.position, GameManager.SFX_Volume);
            PlayerData.StatPoints++;
            PlayerData.SkillPoints++;            
        }     
    }





    public Class GetClass() {
        return PlayerData.Class;
    }
    public int Getlvl() {
        return PlayerData.lvl;
    }
    public int GetExp() {
        return PlayerData.exp;
    }

    public int GetNextLvlExp() {
        return NextLevelExp;
    }

    public CharacterDataStruct GetPlayerData() {
        return PlayerData;
    }

    public override string GetName() {
        return Name;
    }
}
