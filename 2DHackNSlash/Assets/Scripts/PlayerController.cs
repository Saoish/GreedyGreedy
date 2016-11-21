using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerController : ObjectController {
    private int NextLevelExp = -999;

    public AudioClip hurt;
    public AudioClip crit_hurt;
    public AudioClip die;
    public AudioClip lvlup;

    [HideInInspector]
    private CharacterDataStruct PlayerData;

    [HideInInspector]
    public string Name;

    [HideInInspector]
    public float MaxHealth;
    [HideInInspector]
    public float MaxMana;
    [HideInInspector]
    public float MaxAD;
    [HideInInspector]
    public float MaxMD;
    [HideInInspector]
    public float MaxAttkSpd;
    [HideInInspector]
    public float MaxMoveSpd;
    [HideInInspector]
    public float MaxDefense;

    [HideInInspector]
    public float MaxCritChance; //Percantage
    [HideInInspector]
    public float MaxCritDmgBounus; //Percantage
    [HideInInspector]
    public float MaxLPH;
    [HideInInspector]
    public float MaxMPH;


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
    public float CurrMPH;

    Dictionary<string, GameObject> EquipPrefabs;
    

    private ControllerManager CM;
    private SaveLoadManager SLM;
    private PlayerUIController PUIC;

    private GameObject BaseModel;

    private GameObject PickedTarget = null;

    public GameObject FirstWeapon;

    [HideInInspector]
    public GameObject SkillTree;

    protected override void Awake() {
        base.Awake();
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;
        if (FirstWeapon != null)
            FirstWeapon.GetComponent<EquipmentController>().InstantiateLoot(transform);
        EquipPrefabs = new Dictionary<string, GameObject>();
        if (transform.parent.name == "MainPlayer") {
            if (ControllerManager.Instance) {
                CM = ControllerManager.Instance;
            } else
                CM = FindObjectOfType<ControllerManager>();
            if (SaveLoadManager.Instance)
                SLM = SaveLoadManager.Instance;
            else
                SLM = FindObjectOfType<SaveLoadManager>();
            PUIC = transform.parent.Find("PlayerUI").GetComponent<PlayerUIController>();
            PlayerData = SLM.LoadPlayerInfo(SLM.SlotIndexToLoad);
        }
        //InitPlayer();
    }

    protected override void Start() {
        base.Start();
        InitPlayer();
    }

    // Update is called once per frame
    void Update() {
        ControlUpdate();
        PickUpInUpdate();
        EquiPrefabsUpdate();
        BaseModelUpdate();
    }

    void FixedUpdate() {
        MoveUpdate();
    }

    void ControlUpdate() {
        if (Stunned) {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
            return;
        } else {
            if (CM != null) {
                AttackVector = CM.AttackVector;
                if (!HasForce()) {
                    MoveVector = CM.MoveVector;
                } else {
                    MoveVector = Vector2.zero;
                }
                Direction = CM.Direction;
            }
        }
    }

    void MoveUpdate() {
        if (MoveVector != Vector2.zero) {
            rb.MovePosition(rb.position + MoveVector * (CurrMoveSpd / 100) * Time.deltaTime);
            //rb.AddForce(MoveVector * (CurrMoveSpd / 100) * rb.drag);
            //rb.velocity = MoveVector * (CurrMoveSpd / 100);
        }
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("LootBox")) {
            PickedTarget = collider.transform.parent.gameObject;
            return;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("LootBox") && PickedTarget!=null) {
            PickedTarget = null;
        }
    }
    //----------public
    //Skills Handling
    public ActiveSkill GetActiveSlotSkillTransform(int Slot) {
        if (PlayerData.ActiveSlotData[Slot] == null ||Actives.childCount == 0)
            return null;
        for (int i = 0; i < Actives.childCount; i++) {
            if (Actives.GetChild(i).GetComponent<ActiveSkill>().SD.Name == PlayerData.ActiveSlotData[Slot].Name)
                return Actives.GetChild(i).GetComponent<ActiveSkill>();
        }
        return null;
    }

    //EXP handling
    public void AddEXP(int exp) {
        if (PlayerData.lvl < LvlExpModule.LvlCap) {
            PlayerData.exp += exp;
            CheckLevelUp();
            //if(PUIC)
            //    PUIC.UpdateExpBar();
        }
        SLM.SaveCurrentPlayerInfo();
    }

    //Combat
    override public Value AutoAttackDamageDeal(float TargetDefense) {
        Value dmg = Value.CreateValue(0,0,false,GetComponent<ObjectController>());
        if (UnityEngine.Random.value < (CurrCritChance / 100)) {
            dmg.Amount += CurrAD * (CurrCritDmgBounus / 100);
            dmg.Amount += CurrMD * (CurrCritDmgBounus / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount = CurrAD + CurrMD;
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (TargetDefense / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;
        return dmg;
    }

    override public void DeductHealth(Value dmg) {
        if (CurrHealth - dmg.Amount <= 0) {
            CurrHealth -= dmg.Amount;
            IC.UpdateHealthBar();
            IC.PopUpText(dmg);
            DieUpdate();
            return;
        }
        if (dmg.IsCrit) {
            Animator Anim = GetComponent<Animator>();
            Anim.SetFloat("PhysicsSpeedFactor", GetPhysicsSpeedFactor());
            Anim.Play("crit");
            AudioSource.PlayClipAtPoint(crit_hurt, transform.position, GameManager.SFX_Volume);
        }else {
            AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        }
        CurrHealth -= dmg.Amount;
        if (transform.parent.tag == "MainPlayer") {
            PUIC.UpdateHealthManaBar();
        } else {
            IC.UpdateHealthBar();
        }
        IC.PopUpText(dmg);
    }

    public override void DeductMana(Value mana_cost) {
        if (CurrMana - mana_cost.Amount >= 0)//Double check
            CurrMana -= mana_cost.Amount;
        if (transform.parent.tag == "MainPlayer") {
            PUIC.UpdateHealthManaBar();
        }
    }

    public override void HealHP(Value heal_hp) {
        if (CurrHealth < MaxHealth && CurrHealth + heal_hp.Amount <= MaxHealth) {
            CurrHealth += heal_hp.Amount;
            IC.PopUpText(heal_hp);
        } else if (CurrHealth < MaxHealth && CurrHealth + heal_hp.Amount > MaxHealth) {
            heal_hp.Amount = MaxHealth - CurrHealth;
            CurrHealth += heal_hp.Amount;
            IC.PopUpText(heal_hp);
        }
        if (PUIC != null)
            PUIC.UpdateHealthManaBar();
        else
            IC.UpdateHealthBar();
    }

    public override void HealMana(Value heal_mana) {
        if (CurrMana < MaxMana && CurrMana + heal_mana.Amount <= MaxMana) {
            CurrMana += heal_mana.Amount;
            //IC.PopUpHeal(heal_mana);
        } else if (CurrMana < MaxMana && CurrMana + heal_mana.Amount > MaxMana) {
            heal_mana.Amount = MaxMana - CurrMana;
            CurrHealth += heal_mana.Amount;
            //IC.PopUpHeal(heal_mana);
        }
        if (PUIC != null)
            PUIC.UpdateHealthManaBar();
        else
            IC.UpdateHealthBar();
    }


    //Animation Handling
    override public float GetMovementAnimSpeed() {
        return (CurrMoveSpd/100) / (movement_animation_interval);
    }

    override public float GetAttackAnimSpeed() {
        return (CurrAttkSpd/100) / (attack_animation_interval);
    }

    override public float GetPhysicsSpeedFactor() {
        if (!Attacking) {
            if (CurrMoveSpd < 100)
                return 1 + CurrMoveSpd / 100;
            else if (CurrMoveSpd > 100)
                return 1 - CurrMoveSpd / 100;
            else
                return 1;
        } else {
            if (CurrAttkSpd < 100)
                return 1 + CurrAttkSpd / 100;
            else if (CurrMoveSpd > 100)
                return 1 - CurrAttkSpd / 100;
            else
                return 1;
        }
    }

    //Equipment/Inventory Handling
    public bool InventoryIsFull() {
        return FirstAvailbleInventorySlot() == PlayerData.Inventory.Length;
    }


    public WeaponController GetEquippedWeaponController() {
        //Debug.Log("Player version");
        if (GetEquippedItem("Weapon") == null)
            return null;
        return transform.Find(GetEquippedItem("Weapon").Name).GetComponent<WeaponController>();
    }

    public Equipment GetEquippedItem(string Slot) {
        return PlayerData.Equipments[Slot];
    }
    public Equipment GetInventoryItem(int Slot) {
        return PlayerData.Inventory[Slot];
    }

    public bool Compatible(Equipment E) {
        if (E == null)
            return false;
        if (E.Class == "All")//Trinket
            return PlayerData.lvl >= E.LvlReq;
        return (PlayerData.lvl >= E.LvlReq && PlayerData.Class == E.Class);
    }

    public int FirstAvailbleInventorySlot() {
        for(int i = 0; i < PlayerData.Inventory.Length; i++) {
            if (PlayerData.Inventory[i] == null)
                return i;
        }
        return PlayerData.Inventory.Length;
    }

    public void Equip(Equipment E) {
        PlayerData.Equipments[E.Type] = E;
        GameObject equipPrefab = EquipmentController.ObtainPrefab(E, transform);
        EquipPrefabs[E.Type] = equipPrefab;
        UpdateStats();
        SLM.SaveCurrentPlayerInfo();
    }

    public void UnEquip(string Slot) {
        Destroy(EquipPrefabs[Slot]);
        PlayerData.Equipments[Slot] = null;
        UpdateStats();
        SLM.SaveCurrentPlayerInfo();
    }

    public void AddToInventory(int Slot, Equipment E) {
        PlayerData.Inventory[Slot] = E;
        SLM.SaveCurrentPlayerInfo();
    }

    public void RemoveFromInventory(int Slot, Equipment E) {
        PlayerData.Inventory[Slot] = null;
        SLM.SaveCurrentPlayerInfo();
    }

    //-------private
    void InitPlayer() {
        InstaniateEquipmentModel();

        if (PlayerData.lvl < LvlExpModule.LvlCap)
            NextLevelExp = LvlExpModule.GetRequiredExp(PlayerData.lvl + 1);

        InitMaxStats();
        InitSkillTree();
        InitOnCallEvent();
        InitPassives();
        InitCurrStats();


        //PUIC.UpdateExpBar();
    }

    void InitPassives() {
        //Debug.Log(Passives.GetComponentsInChildren<PassiveSkill>().Length);
        foreach (var passive in Passives.GetComponentsInChildren<PassiveSkill>()) {
            passive.ApplyPassive();
        }
    }

    void InitSkillTree() {
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
                GameObject SkillObject = Instantiate(Resources.Load("SkillPrefabs/" + STC.SkillTree[i].Name)) as GameObject;
                if (SkillObject.transform.GetComponent<Skill>().GetType().IsSubclassOf(typeof(ActiveSkill)))
                    SkillObject.transform.SetParent(Actives);
                else if (SkillObject.transform.GetComponent<Skill>().GetType().IsSubclassOf(typeof(PassiveSkill))) {
                    SkillObject.transform.SetParent(Passives);
                }
                SkillObject.name = STC.SkillTree[i].Name;
                SkillObject.GetComponent<Skill>().InitSkill(PlayerData.SkillTreelvls[i]);
            }
        }
        if (PUIC) {//MainPlayer UI

        }
    }

    void InitMaxStats() {
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
        MaxMPH = PlayerData.BaseMPH;
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
                MaxMPH += e.Value.AddMPH;
            }           
        }
    }

    void InitCurrStats() {
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
        CurrMPH = MaxMPH;
    }

    void BaseModelUpdate() {
        Animator BaseModelAnim = BaseModel.GetComponent<Animator>();
        if (CM != null) {
            BaseModelAnim.SetInteger("Direction", Direction);
            BaseModelAnim.speed = GetMovementAnimSpeed();
        }
    }

    void EquiPrefabsUpdate() {
        if (CM != null) {
            foreach(var e_prefab in EquipPrefabs.Values) {
                if(e_prefab!=null)
                    e_prefab.GetComponent<EquipmentController>().EquipUpdate(Direction, AttackVector);
            }
        }      
    }

    //-------helper
    void UpdateStats() {
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
        MaxMPH = PlayerData.BaseMPH;
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
                MaxMPH += e.Value.AddMPH;
            }
        }

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
        CurrMPH = MaxMPH;
    }

    void InitOnCallEvent() {
        ON_DMG_DEAL = null;
        ON_HEALTH_UPDATE = null;
        ON_MANA_UPDATE = null;
    }

    void InstaniateEquipmentModel() {
        if (PlayerData.Class == "Warrior") {
            BaseModel = Instantiate(Resources.Load("BaseModelPrefabs/Red Ghost"), transform) as GameObject;
            BaseModel.name = "Red Ghost";
            BaseModel.transform.position = transform.position + BaseModel.transform.position;
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

    void PickUpInUpdate() {
        if (PickedTarget != null && CM.AllowControlUpdate) {
            PUIC.transform.Find("PickUpNotify").gameObject.SetActive(true);
            if (Input.GetKeyDown(CM.Interact) || Input.GetKeyDown(CM.J_A)) {
                if (InventoryIsFull()) {
                    Debug.Log("Your inventory is full!");
                } else {
                    int InventoryIndex = FirstAvailbleInventorySlot();
                    AddToInventory(InventoryIndex, PickedTarget.GetComponent<EquipmentController>().E);
                    Destroy(PickedTarget);
                    PickedTarget = null;
                }
            }
        } else
            PUIC.transform.Find("PickUpNotify").gameObject.SetActive(false);
    }

    void CheckLevelUp() {
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

    void DieUpdate() {
        if (CurrHealth <= 0) {//Insert dead animation here
            //GetComponent<DropList>().SpawnLoots(); //Added for PVP later
            Alive = false;
            Destroy(gameObject);
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
    override public float GetMaxMPH() {
        return MaxMPH;
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
    override public float GetCurrMPH() {
        return CurrMPH;
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
    override public void SetMaxMPH(float mph) {
        MaxMPH = mph;
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
    override public void SetCurrMPH(float mph) {
        CurrMPH = mph;
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
