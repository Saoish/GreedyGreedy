using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class PlayerController : ObjectController {
    protected int NextLevelExp = -999;

    public AudioClip hurt;
    public AudioClip crit_hurt;
    public AudioClip lvlup;

    protected CharacterDataStruct PlayerData;

    protected string Name;

    protected float MaxHealth;
    protected float MaxMana;
    protected float MaxAD;
    protected float MaxMD;
    protected float MaxAttkSpd;
    protected float MaxMoveSpd;
    protected float MaxDefense;

    [HideInInspector]
    public float MaxCritChance; //Percantage
    [HideInInspector]
    public float MaxCritDmgBounus; //Percantage
    [HideInInspector]
    public float MaxLPH;
    [HideInInspector]
    public float MaxManaRegen;


    public float CurrHealth;
    public float CurrMana;
    public float CurrAD;
    public float CurrMD;
    public float CurrAttkSpd;
    public float CurrMoveSpd;
    public float CurrDefense;
    public float CurrCritChance;    
    public float CurrCritDmgBounus;    
    public float CurrLPH;    
    public float CurrManaRegen;

    protected Dictionary<string, GameObject> EquipPrefabs;

    protected GameObject BaseModel;

    protected WeaponController WC;

    [HideInInspector]
    public GameObject SkillTree;

    protected override void Awake() {
        base.Awake();
        EquipPrefabs = new Dictionary<string, GameObject>();
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
        ManaRegen();
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
        if (CurrHealth - dmg.Amount <= 0 && Alive) {
            CurrHealth -= dmg.Amount;
            IC.PopUpText(dmg);
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
            return;
        }
        if (dmg.Type == -1) {//Dot no sound update
        } else if (dmg.IsCrit) {
            Animator Anim = VisualHolder.GetComponent<Animator>();
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
            AudioSource.PlayClipAtPoint(crit_hurt, transform.position, GameManager.SFX_Volume);
        }
        if (dmg.IsCrit) {
            Animator Anim = VisualHolder.GetComponent<Animator>();
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
            if(dmg.Type!=-1)
                AudioSource.PlayClipAtPoint(crit_hurt, transform.position, GameManager.SFX_Volume);
        } else {
            if (dmg.Type != -1)
                AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        }
        if (CurrHealth - dmg.Amount <= 0 && Alive) {
            IC.PopUpText(dmg);
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
        } else {
            CurrHealth -= dmg.Amount;
            IC.PopUpText(dmg);
        }
    }

    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical("Body Parts Explode", Layer.Ground);

    }

    //-------private
    protected void ReloadWeaponModel() {
        if (PlayerData.Equipments["Weapon"] != null) {
            Destroy(EquipPrefabs["Weapon"]);
            EquipPrefabs["Weapon"] = EquipmentController.ObtainPrefab(PlayerData.Equipments["Weapon"], transform);
            FetchWC();
        }
    }

    private void FetchWC() {
        if (PlayerData.Equipments["Weapon"] == null) {
            WC = null;
            return;
        }
        WC = EquipPrefabs["Weapon"].GetComponent<WeaponController>();
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
        if (PlayerData.Class == "Warrior") {
            SkillTree = Instantiate(Resources.Load("SkillPrefabs/WarriorSkillTree"), transform) as GameObject;
            SkillTree.name = "SkillTree";
        }
        else if(PlayerData.Class == "Mage") {

        }else if(PlayerData.Class == "Rogue") {

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
        MaxHealth = PlayerData.BaseHealth;
        MaxMana = PlayerData.BaseMana;
        MaxAD = PlayerData.BaseAD;
        MaxMD = PlayerData.BaseMD;
        MaxAttkSpd = PlayerData.BaseAttkSpd;
        MaxMoveSpd = PlayerData.BaseMoveSpd;
        MaxDefense = PlayerData.BaseDefense;
        MaxCritChance = PlayerData.BaseCritChance;
        MaxCritDmgBounus = PlayerData.BaseCritDmgBounus;
        MaxLPH = PlayerData.BaseLPH;
        MaxManaRegen = PlayerData.BaseManaRegen;
        foreach (var e in PlayerData.Equipments) {
            if (e.Value != null) {
                MaxHealth += e.Value.AddHealth;
                MaxMana += e.Value.AddMana;
                MaxAD += e.Value.AddAD;
                MaxMD += e.Value.AddMD;
                MaxAttkSpd += e.Value.AddAttkSpd;
                MaxMoveSpd += e.Value.AddMoveSpd;
                MaxDefense += e.Value.AddDefense;
                MaxCritChance += e.Value.AddCritChance;
                MaxCritDmgBounus += e.Value.AddCritDmgBounus;
                MaxLPH += e.Value.AddLPH;
                MaxManaRegen += e.Value.AddManaRegen;
            }           
        }
    }

    protected void InitCurrStats() {
        CurrHealth = MaxHealth;
        CurrMana = MaxMana;
        CurrAD = MaxAD;
        CurrMD = MaxMD;
        CurrAttkSpd = MaxAttkSpd;
        CurrMoveSpd = MaxMoveSpd;
        CurrDefense = MaxDefense;
        CurrCritChance = MaxCritChance;
        CurrCritDmgBounus = MaxCritDmgBounus;
        CurrLPH = MaxLPH;
        CurrManaRegen = MaxManaRegen;
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
        MaxHealth = PlayerData.BaseHealth;
        MaxMana = PlayerData.BaseMana;
        MaxAD = PlayerData.BaseAD;
        MaxMD = PlayerData.BaseMD;
        MaxAttkSpd = PlayerData.BaseAttkSpd;
        MaxMoveSpd = PlayerData.BaseMoveSpd;
        MaxDefense = PlayerData.BaseDefense;
        MaxCritChance = PlayerData.BaseCritChance;
        MaxCritDmgBounus = PlayerData.BaseCritDmgBounus;
        MaxLPH = PlayerData.BaseLPH;
        MaxManaRegen = PlayerData.BaseManaRegen;
        foreach (var e in PlayerData.Equipments) {
            if (e.Value != null) {
                MaxHealth += e.Value.AddHealth;
                MaxMana += e.Value.AddMana;
                MaxAD += e.Value.AddAD;
                MaxMD += e.Value.AddMD;
                MaxAttkSpd += e.Value.AddAttkSpd;
                MaxMoveSpd += e.Value.AddMoveSpd;
                MaxDefense += e.Value.AddDefense;
                MaxCritChance += e.Value.AddCritChance;
                MaxCritDmgBounus += e.Value.AddCritDmgBounus;
                MaxLPH += e.Value.AddLPH;
                MaxManaRegen += e.Value.AddManaRegen;
            }
        }

        ReloadWeaponModel();
        InitOnCallEvent();
        InitPassives();

        if (CurrHealth>MaxHealth)
            CurrHealth = MaxHealth;
        if(CurrMana>MaxMana)
            CurrMana = MaxMana;
        CurrAD = MaxAD;
        CurrMD = MaxMD;
        CurrAttkSpd = MaxAttkSpd;
        CurrMoveSpd = MaxMoveSpd;
        CurrDefense = MaxDefense;
        CurrCritChance = MaxCritChance;
        CurrCritDmgBounus = MaxCritDmgBounus;
        CurrLPH = MaxLPH;
        CurrManaRegen = MaxManaRegen;
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
        if (PlayerData.Class == "Warrior") {
            BaseModel = Instantiate(Resources.Load("BaseModelPrefabs/Red Ghost"), VisualHolder) as GameObject;
            BaseModel.name = "Red Ghost";
            BaseModel.transform.position = VisualHolder.position + BaseModel.transform.position;
            BaseModel.transform.GetComponent<SpriteRenderer>().sortingOrder = Layer.OJ;
        }
        else if(PlayerData.Class == "Mage") {

        }
        else if(PlayerData.Class == "Rogue") {

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
            CurrHealth = MaxHealth;
            CurrMana = MaxMana;
            NextLevelExp = LvlExpModule.GetRequiredExp(PlayerData.lvl + 1);
            AudioSource.PlayClipAtPoint(lvlup, transform.position, GameManager.SFX_Volume);
            PlayerData.StatPoints++;
            PlayerData.SkillPoints++;            
        }     
    }

    override public float GetMaxHealth() {
        return MaxHealth;
    }
    override public float GetMaxMana() {
        return MaxMana;
    }
    override public float GetMaxAD() {
        return MaxAD;
    }
    override public float GetMaxMD() {
        return MaxMD;
    }
    override public float GetMaxAttkSpd() {
        return MaxAttkSpd;
    }
    override public float GetMaxMoveSpd() {
        return MaxMoveSpd;
    }
    override public float GetMaxCritChance() {
        return MaxCritChance;
    }
    override public float GetMaxCritDmgBounus() {
        return MaxCritDmgBounus;
    }
    override public float GetMaxLPH() {
        return MaxLPH;
    }
    override public float GetMaxManaRegen() {
        return MaxManaRegen;
    }
    override public float GetMaxDefense() {
        return MaxDefense;
    }





    override public float GetCurrHealth() {
        return CurrHealth;
    }
    override public float GetCurrMana() {
        return CurrMana;
    }
    override public float GetCurrAD() {
        return CurrAD;
    }
    override public float GetCurrMD() {
        return CurrMD;
    }
    override public float GetCurrAttkSpd() {
        return CurrAttkSpd;
    }
    override public float GetCurrMoveSpd() {
        return CurrMoveSpd;
    }
    override public float GetCurrCritChance() {
        return CurrCritChance;
    }
    override public float GetCurrCritDmgBounus() {
        return CurrCritDmgBounus;
    }
    override public float GetCurrLPH() {
        return CurrLPH;
    }
    override public float GetCurrManaRegen() {
        return CurrManaRegen;
    }
    override public float GetCurrDefense() {
        return CurrDefense;
    }



    override public void SetMaxHealth(float health) {
        MaxHealth = health;
    }
    override public void SetMaxMana(float mana) {
        MaxMana = mana;
    }
    override public void SetMaxAD(float ad) {
        MaxAD = ad;
    }
    override public void SetMaxMD(float md) {
        MaxMD = md;
    }
    override public void SetMaxAttkSpd(float attkspd) {
        MaxAttkSpd = attkspd;
    }
    override public void SetMaxMoveSpd(float movespd) {
        MaxMoveSpd = movespd;
    }
    override public void SetMaxCritChance(float critchance) {
        MaxCritChance = critchance;
    }
    override public void SetMaxCritDmgBounus(float critdmg) {
        MaxCritDmgBounus = critdmg;
    }
    override public void SetMaxLPH(float lph) {
        MaxLPH = lph;
    }
    override public void SetMaxManaRegen(float mph) {
        MaxManaRegen = mph;
    }
    override public void SetMaxDefense(float defense) {
        MaxDefense = defense;
    }


    override public void SetCurrHealth(float health) {
        CurrHealth = health;
    }
    override public void SetCurrMana(float mana) {
        CurrMana = mana;
    }
    override public void SetCurrAD(float ad) {
        CurrAD = ad;
    }
    override public void SetCurrMD(float md) {
        CurrMD = md;
    }
    override public void SetCurrAttkSpd(float attkspd) {
        CurrAttkSpd = attkspd;
    }
    override public void SetCurrMoveSpd(float movespd) {
        CurrMoveSpd = movespd;
    }
    override public void SetCurrCritChance(float critchance) {
        CurrCritChance = critchance;
    }
    override public void SetCurrCritDmgBounus(float critdmg) {
        CurrCritDmgBounus = critdmg;
    }
    override public void SetCurrLPH(float lph) {
        CurrLPH = lph;
    }
    override public void SetCurrManaRegen(float mph) {
        CurrManaRegen = mph;
    }
    override public void SetCurrDefense(float defense) {
        CurrDefense = defense;
    }




    public override string GetName() {
        return Name;
    }

    public string GetClass() {
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
}
