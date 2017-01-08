using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GreedyNameSpace;
using Networking;
using Networking.Data;

public abstract class Player : ObjectController {
    protected int NextLevelExp = -999;

    public AudioClip LevelUpSFX;

    [HideInInspector]
    public PlayerData PlayerData;

    protected List<GameObject> EquipPrefabs;

    protected GameObject BaseModel;

    protected string Name;

    List<Bounus> Bounuses = new List<Bounus>();

    //public abstract PlayerController Instantiate(PlayerData PlayerData);

    protected override void Awake() {
        base.Awake();
        MaxStats = new Stats();    
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
        if (PlayerData.Equipments[(int)EQUIPTYPE.Weapon].isNull) {
            return null;
        }
        return EquipPrefabs[(int)EQUIPTYPE.Weapon].GetComponent<WeaponController>();
    }

    //Combat
    override public void DeductHealth(Damage dmg) {
        IC.PopUpText(dmg);
        if (dmg.Crit) {
            Animator Anim = VisualHolder.GetComponent<Animator>();
            Anim.Play("crit");
            //if (dmg.SFX_Update)
            //    AudioSource.PlayClipAtPoint(crit_hurt, transform.position, GameManager.SFX_Volume);
        }
        //} else {
        //    if (dmg.SFX_Update)
        //        AudioSource.PlayClipAtPoint(hurt, transform.position, GameManager.SFX_Volume);
        //}
        if (CurrStats.Get(STATSTYPE.HEALTH) - dmg.Amount <= 0 && Alive) {
            ON_DEATH_UPDATE += Die;
            ON_DEATH_UPDATE();
            ON_DEATH_UPDATE -= Die;
        } else {
            CurrStats.Dec(STATSTYPE.HEALTH, dmg.Amount);
        }
    }

    protected override void Die() {
        base.Die();
        ActiveOutsideVFXPartical(DieVFX);

    }

    //-------private

    protected void BaseModelUpdate() {
        Animator BaseModelAnim = BaseModel.GetComponent<Animator>();
        BaseModelAnim.SetInteger("Direction", Direction);
        BaseModelAnim.speed = GetMovementAnimSpeed();
    }

    protected void EquiPrefabsUpdate() {
        if (!Alive)
            return;
        foreach (var e_prefab in EquipPrefabs) {
            if (e_prefab != null)
                e_prefab.GetComponent<EquipmentController>().EquipUpdate(GetComponent<Player>());
        }
    }

    protected void InstaniateEquipmentModel() {
        BaseModel = Instantiate(Resources.Load("BaseModelPrefabs/BaseModel"), VisualHolder) as GameObject;
        BaseModel.name = "BaseModel";
        BaseModel.GetComponent<SpriteRenderer>().color = new Color(PlayerData.SkinColor.R, PlayerData.SkinColor.G, PlayerData.SkinColor.B);
        BaseModel.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Object;
        BaseModel.transform.localPosition = Vector3.zero;
        EquipPrefabs = new List<GameObject>();
        foreach (var e in PlayerData.Equipments) {
            if (e.isNull) {
                EquipPrefabs.Add(null);
            } else {
                GameObject equipPrefab = EquipmentController.ObtainPrefab(e, transform);
                EquipPrefabs.Add(equipPrefab);
            }
        }
    }





    public CLASS GetClass() {
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

    public PlayerData GetPlayerData() {
        return PlayerData;
    }

    public override string GetName() {
        return Name;
    }

    //PlayerUnique methods

    //Skills Handling
    public int GetAvailableSkillPoints() {
        return PlayerData.SkillPoints;
    }

    public void SetActiveSkillAt(int Slot, Skill ActiveSkill) {
        if (ActiveSkill == null)
            PlayerData.ActiveSlotData[Slot] = "null";
        else
            PlayerData.ActiveSlotData[Slot] = ActiveSkill.Name;
        //CacheManager.SaveCurrentPlayerInfo();
    }

    public ActiveSkill GetActiveSlotSkill(int Slot) {
        if (PlayerData.ActiveSlotData[Slot] == null || Actives.Count == 0)
            return null;
        foreach (ActiveSkill active in Actives)
            if (active.Name == PlayerData.ActiveSlotData[Slot]) {
                return active;
            }
        return null;
    }

    public Skill GetRawSkill(int SkillIndex) {
        return CachedTreeSkills[SkillIndex];
    }

    public Skill GetActualSkill(int SkillIndex) {
        Skill s_return = null;
        System.Type SkillType = CachedTreeSkills[SkillIndex].GetType();
        if (CachedTreeSkills[SkillIndex].GetType().IsSubclassOf(typeof(ActiveSkill))) {
            foreach (ActiveSkill s in Actives)
                if (s.GetType() == SkillType) {
                    s_return = s;
                }
        } else {
            foreach (PassiveSkill s in Passives)
                if (s.GetType() == SkillType)
                    s_return = s;
        }
        return s_return;
    }

    public int GetSkilllvlByIndex(int SkillIndex) {
        return PlayerData.SkillTreelvls[SkillIndex];
    }

    public void LvlUpSkill(int SkillIndex) {
        PlayerData.SkillTreelvls[SkillIndex]++;
        PlayerData.SkillPoints--;
        //CacheManager.SaveCurrentPlayerInfo();
        UpdateSkillTreeState();
        UpdateStats();
    }

    //Equipment/Inventory Handling
    public bool InventoryIsFull() {
        return FirstAvailbleInventorySlot == PlayerData.Inventory.Length;
    }

    public Equipment GetEquippedItem(EQUIPTYPE Slot) {
        return PlayerData.Equipments[(int)Slot];
    }
    public Equipment GetInventoryItem(int Slot) {
        return PlayerData.Inventory[Slot];
    }

    public bool Compatible(Equipment E) {
        if (E == null)
            return false;
        if (E.Class == CLASS.All)//Trinket
            return PlayerData.lvl >= E.LvlReq;
        return (PlayerData.lvl >= E.LvlReq && PlayerData.Class == E.Class);
    }

    public int FirstAvailbleInventorySlot {
        get {
            for (int i = 0; i < PlayerData.Inventory.Length; i++) {
                if (PlayerData.Inventory[i].isNull)
                    return i;
            }
            return PlayerData.Inventory.Length;
        }
    }    

    public virtual void Equip(Equipment E) {
        PlayerData.Equipments[(int)E.EquipType] = E;
        GameObject equipPrefab = EquipmentController.ObtainPrefab(E, transform);
        EquipPrefabs[(int)E.EquipType] = equipPrefab;
        UpdateStats();
        //CacheManager.SaveCurrentPlayerInfo();        
    }

    public virtual void UnEquip(EQUIPTYPE Slot) {
        Destroy(EquipPrefabs[(int)Slot]);
        PlayerData.Equipments[(int)Slot] = new Equipment();
        UpdateStats();
        //CacheManager.SaveCurrentPlayerInfo();
    }

    public virtual void AddToInventory(int Slot, Equipment E) {        
        PlayerData.Inventory[Slot] = E;
        //CacheManager.SaveCurrentPlayerInfo();
    }

    public virtual void RemoveFromInventory(int Slot) {
        PlayerData.Inventory[Slot] = new Equipment();        
        //CacheManager.SaveCurrentPlayerInfo();
    }

    //EXP handling
    public void AddEXP(int exp) {
        if (PlayerData.lvl < LvlExpModule.LvlCap) {
            PlayerData.exp += exp;
            CheckLevelUp();
        }
        //CacheManager.SaveCurrentPlayerInfo();
    }

    protected void InitPlayer() {
        InstaniateEquipmentModel();
        if (PlayerData.lvl < LvlExpModule.LvlCap)
            NextLevelExp = LvlExpModule.GetRequiredExp(PlayerData.lvl + 1);
        InitMaxStats();
        ApplyEquipmentsUtilities();
        InitSkillTree();
        InitOnCallEvent();
        ApplyBounuses();
        InitPassives();
        InitCurrStats();
    }

    protected void InitPassives() {
        foreach (PassiveSkill passive in Passives) {
            passive.ApplyPassive();
        }
    }

    protected void InitSkillTree() {
        if (PlayerData.Class == CLASS.Warrior) {
            GameObject SkillTreeOJ;
            SkillTreeOJ = Resources.Load("SkillPrefabs/WarriorSkillTree") as GameObject;
            CachedTreeSkills = SkillTreeOJ.GetComponent<SkillTreeController>().SkillTree;
        } else if (PlayerData.Class == CLASS.Mage) {

        } else if (PlayerData.Class == CLASS.Rogue) {

        }
        for (int i = 0; i < Patch.SkillTreeSize; i++) {
            CachedTreeSkills[i].GenerateDescription();
            if (CachedTreeSkills[i] != null && PlayerData.SkillTreelvls[i] != 0) {//Does lvl+skill check here
                Skill s = CachedTreeSkills[i].Instantiate();
                s.InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                if (s.GetType().IsSubclassOf(typeof(PassiveSkill)))
                    Passives.Add((PassiveSkill)s);
                else
                    Actives.Add((ActiveSkill)s);
            }
        }
    }

    protected void InitMaxStats() {
        Name = PlayerData.Name;
        for (int i = 0; i < Stats.Size; i++) {
            MaxStats.Set(i, PlayerData.BaseStats.Get(i));
        }
    }

    protected void ResetBounuses() {
        if (Bounuses.Count > 0) {
            foreach (Bounus b in Bounuses)
                b.RemoveBounus();
        }
        Bounuses = new List<Bounus>();
    }

    protected void ApplyEquipmentsUtilities() {
        Dictionary<EQUIPSET, int> Sets = new Dictionary<EQUIPSET, int>();
        foreach (var e in PlayerData.Equipments) {
            if (e != null) {
                for (int i = 0; i < Stats.Size; i++) {
                    MaxStats.Add(i, e.Stats.Get(i));
                }
                if (e.Set != EQUIPSET.None) {
                    if (!Sets.ContainsKey(e.Set))
                        Sets.Add(e.Set, 1);
                    else
                        Sets[e.Set]++;
                }
            }
        }
        foreach (var set in Sets) {
            Set s = ((GameObject)Resources.Load("SetPrefabs/" + set.Key.ToString())).GetComponent<Set>();
            foreach (Bounus b in s.Bounuses) {
                if (set.Value >= b.condiction) {
                    Bounuses.Add(new Bounus(b));
                }
            }
        }
    }

    protected void ApplyBounuses() {
        foreach (Bounus b in Bounuses)
            b.ApplyBounus(GetComponent<ObjectController>());
    }

    protected void InitCurrStats() {
        for (int i = 0; i < Stats.Size; i++) {
            CurrStats.Set(i, MaxStats.Get(i));
        }
    }

    //-------helper
    protected void UpdateStats() {
        ResetBounuses();
        InitMaxStats();
        ApplyEquipmentsUtilities();
        ReloadWeaponWC();
        InitOnCallEvent();
        ApplyBounuses();
        InitPassives();

        for (int i = 0; i < Stats.Size; i++) {
            if ((int)STATSTYPE.HEALTH == i) {
                if (CurrStats.Get(i) > MaxStats.Get(i))
                    CurrStats.Set(i, MaxStats.Get(i));
            } else if ((int)STATSTYPE.ESSENSE == i) {
                if (CurrStats.Get(i) > MaxStats.Get(i))
                    CurrStats.Set(i, MaxStats.Get(i));
            } else {
                CurrStats.Set(i, MaxStats.Get(i));
            }
        }
    }

    protected void UpdateSkillTreeState() {
        for (int i = 0; i < Patch.SkillTreeSize; i++) {
            if (CachedTreeSkills[i].GetType().IsSubclassOf(typeof(ActiveSkill))) {
                ActiveSkill active = GetActive(CachedTreeSkills[i].GetType());
                if (active && PlayerData.SkillTreelvls[i] != 0)
                    active.InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                else if (active && PlayerData.SkillTreelvls[i] == 0) {
                    Actives.Remove(active);
                    active.Delete();
                } else if (active == null && PlayerData.SkillTreelvls[i] != 0) {
                    Skill s = CachedTreeSkills[i].Instantiate();
                    s.InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                    Actives.Add((ActiveSkill)s);
                }
            } else {
                PassiveSkill passive = GetPassive(CachedTreeSkills[i].GetType());
                if (passive && PlayerData.SkillTreelvls[i] != 0)
                    passive.InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                else if (passive && PlayerData.SkillTreelvls[i] == 0) {
                    Passives.Remove(passive);
                    passive.Delete();
                } else if (passive == null && PlayerData.SkillTreelvls[i] != 0) {
                    Skill s = CachedTreeSkills[i].Instantiate();
                    s.InitSkill(GetComponent<ObjectController>(), PlayerData.SkillTreelvls[i]);
                    Passives.Add((PassiveSkill)s);
                }
            }
        }
    }

    protected void InitOnCallEvent() {
        ON_DMG_DEAL = null;
        ON_DMG_TAKEN = null;
        ON_HEALTH_GAIN = null;
        ON_ESSENSE_COST = null;
        ON_ESSENSE_GAIN = null;
        ON_DEATH_UPDATE = null;
    }

    protected void CheckLevelUp() {
        if (PlayerData.lvl >= LvlExpModule.LvlCap)
            return;
        if (PlayerData.exp >= NextLevelExp) {
            PlayerData.lvl++;
            PlayerData.exp = 0;
            CurrStats.Set(STATSTYPE.HEALTH, MaxStats.Get(STATSTYPE.HEALTH));
            CurrStats.Set(STATSTYPE.ESSENSE, MaxStats.Get(STATSTYPE.ESSENSE));
            NextLevelExp = LvlExpModule.GetRequiredExp(PlayerData.lvl + 1);
            AudioSource.PlayClipAtPoint(LevelUpSFX, transform.position, GameManager.SFX_Volume);
            PlayerData.SkillPoints++;
        }
    }

    protected void ReloadWeaponWC() {
        if (!PlayerData.Equipments[(int)EQUIPTYPE.Weapon].isNull) {
            EquipPrefabs[(int)EQUIPTYPE.Weapon].GetComponent<WeaponController>().EssenseCost = ((GameObject)Resources.Load("EquipmentPrefabs/" + PlayerData.Equipments[(int)EQUIPTYPE.Weapon].Name)).GetComponent<WeaponController>().EssenseCost;
        }
    }
}
