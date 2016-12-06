using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GreedyNameSpace;

[System.Serializable]
public struct EquipmentField {
    public StatsType type;
    //public int DP;
    public Vector2 stats_range;
}
[System.Serializable]
public struct RandomFields {
    public int Choose;
    public List<EquipmentField> Fields;
}

public class EquipmentController : MonoBehaviour {
    public string Name;
    public Class Class;
    public EquipType EquipType;
    public EquipSet Set;
    public string Description;

    [HideInInspector]
    Rarity Rarity;
    [HideInInspector]
    int Itemlvl;
    public List<EquipmentField> SolidFields;
    public RandomFields RandomFields;
    

    [HideInInspector]
    public Equipment E;

    private Animator Anim;
    // Use this for initialization
    void Awake() {
        InstantiateEquipmentData();
        Anim = GetComponent<Animator>();
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }


    public void EquipUpdate(PlayerController PC) {
        if (E.EquipType == EquipType.Trinket) {//No update for trinket for now
            return;
        }
        Anim.SetInteger("Direction", PC.Direction);
        if (PC.Attacking) {
            Anim.speed = PC.GetAttackAnimSpeed();
        } else {
            Anim.speed = PC.GetMovementAnimSpeed();
        }
        if (E.EquipType == EquipType.Weapon) {//Weapon animation speed is controlled by AttackCollider
            if (PC.AttackVector != Vector2.zero) {
                Anim.SetBool("IsAttacking",true);
                Anim.SetInteger("Direction", PC.Direction);
            } else {
                Anim.SetBool("IsAttacking", false);
            }
            SpriteRenderer WeaponSpriteRenderer = GetComponent<SpriteRenderer>();
            if (PC.Direction == 3) {
                WeaponSpriteRenderer.sortingLayerName = Layer.Object;
                WeaponSpriteRenderer.sortingOrder = -1;
            } else {
                WeaponSpriteRenderer.sortingLayerName = Layer.Equip;
                GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        } else {//Helmet,Chest,Shackle
            Anim.speed = PC.GetMovementAnimSpeed();
        }
    }

    static public GameObject ObtainPrefabForCharacterSelection(Equipment E,Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        equipPrefab.GetComponent<EquipmentController>().E = E;
        Destroy(equipPrefab.GetComponent<Rigidbody2D>());
        Destroy(equipPrefab.GetComponent<BoxCollider2D>());
        equipPrefab.name = E.Name;
        equipPrefab.transform.position = parent.transform.position + equipPrefab.transform.position;
        Destroy(equipPrefab.transform.Find("LootBox").gameObject);
        Destroy(equipPrefab.transform.Find("Icon").gameObject);
        equipPrefab.transform.parent = parent;
        equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Equip;
        if (E.EquipType == GreedyNameSpace.EquipType.Weapon) {
            equipPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
        } 
        return equipPrefab;
    }

    static public GameObject ObtainPrefab(Equipment E,Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        equipPrefab.GetComponent<EquipmentController>().E = E;
        Destroy(equipPrefab.GetComponent<Rigidbody2D>());
        Destroy(equipPrefab.GetComponent< BoxCollider2D>());
        equipPrefab.name = E.Name;
        equipPrefab.transform.position = parent.transform.position + equipPrefab.transform.position;
        Destroy(equipPrefab.transform.Find("LootBox").gameObject);
        Destroy(equipPrefab.transform.Find("Icon").gameObject);
        equipPrefab.transform.parent = parent.GetComponent<PlayerController>().GetVisualHolderTransform();
        equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Equip;
        if (E.EquipType == GreedyNameSpace.EquipType.Weapon) {
            //equipPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if (equipPrefab.transform.Find("MeleeAttackCollider") != null) {
                parent.GetComponent<PlayerController>().SwapMeleeAttackCollider(equipPrefab.transform.Find("MeleeAttackCollider"));
            }
        }
        //} else if (E.Type == "Trinket")
        //    equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Equip;
        //else
        //    equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Equip;
        return equipPrefab;
    }

    static public GameObject ObtainEquippedIcon(Equipment E, Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        GameObject equipIcon = equipPrefab.transform.Find("Icon").gameObject;
        equipIcon.SetActive(true);
        equipIcon.name = E.Name;
        equipIcon.transform.position = parent.transform.position + equipIcon.transform.position;
        equipIcon.transform.parent = parent;
        Destroy(equipPrefab);
        E.Description = equipPrefab.GetComponent<EquipmentController>().E.Description;
        return equipIcon;
    }

    static public GameObject ObtainInventoryIcon(Equipment E, Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        GameObject equipIcon = equipPrefab.transform.Find("Icon").gameObject;
        equipIcon.SetActive(true);
        equipIcon.name = E.Name;
        equipIcon.transform.position = parent.transform.position + equipIcon.transform.position;
        equipIcon.transform.parent = parent;
        equipIcon.transform.localScale = new Vector2(500, 500);
        Destroy(equipPrefab);
        E.Description = equipPrefab.GetComponent<EquipmentController>().E.Description;
        return equipIcon;
    }

    public void InstantiateLootAt(Vector3 Position, Vector2 ILvlRange, float RarityMod) {
        InstantiateEquipmentData();
        GameObject Loot = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name), Position, gameObject.transform.rotation) as GameObject;
        Loot.layer = LayerMask.NameToLayer("Loot");
        Loot.transform.Find("LootBox").gameObject.layer = LayerMask.NameToLayer("LootBox");
        Loot.name = E.Name;
        Destroy(Loot.transform.Find("Icon").gameObject);
        Loot.transform.Find("LootBox").gameObject.SetActive(true);
        Loot.GetComponent<EquipmentController>().RandomlizeEquip(ILvlRange, RarityMod);
        Loot.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Loot; //Subject to change
        Loot.GetComponent<BoxCollider2D>().enabled = true;
        Text E_NameText = Loot.transform.Find("LootBox/UI/Name").GetComponent<Text>();
        switch (Loot.GetComponent<EquipmentController>().Rarity) {
            case Rarity.Common:
                E_NameText.color = MyColor.White;
                break;
            case Rarity.Fine:
                E_NameText.color = MyColor.Cyan;
                break;
            case Rarity.Perfect:
                E_NameText.color = MyColor.Yellow;
                break;
            case Rarity.Mythic:
                E_NameText.color = MyColor.Purple;
                break;
            case Rarity.Legendary:
                E_NameText.color = MyColor.Orange;
                break;
        }
        E_NameText.text = E.Name;
    }

    public void InstantiateLoot(Transform T, Vector2 ILvlRange, float RarityMod) {
        InstantiateEquipmentData();
        GameObject Loot = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name), T.position, T.rotation) as GameObject;
        Loot.layer = LayerMask.NameToLayer("Loot");
        Loot.transform.Find("LootBox").gameObject.layer = LayerMask.NameToLayer("LootBox");
        Loot.name = E.Name;
        Destroy(Loot.transform.Find("Icon").gameObject);
        Loot.transform.Find("LootBox").gameObject.SetActive(true);
        Loot.GetComponent<EquipmentController>().RandomlizeEquip(ILvlRange,RarityMod);
        Loot.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Loot; //Subject to change
        Loot.GetComponent<BoxCollider2D>().enabled = true;
        Text E_NameText = Loot.transform.Find("LootBox/UI/Name").GetComponent<Text>();

        switch (Loot.GetComponent<EquipmentController>().E.Rarity) {
            case Rarity.Common:
                E_NameText.color = MyColor.White;
                break;
            case Rarity.Fine:
                E_NameText.color = MyColor.Cyan;
                break;
            case Rarity.Perfect:
                E_NameText.color = MyColor.Yellow;
                break;
            case Rarity.Mythic:
                E_NameText.color = MyColor.Purple;
                break;
            case Rarity.Legendary:
                E_NameText.color = MyColor.Orange;
                break;
        }
        E_NameText.text = E.Name;
    }

    //Could be public in future
    void RandomlizeEquip(Vector2 ILvlRange, float RarityMod) {
        E.Rarity = GetRandomRarity(RarityMod);
        E.Itemlvl = (int)E.Rarity + GetRandomIlvl(ILvlRange);
        E.LvlReq = E.Itemlvl - (int)E.Rarity;
        GenerateSolidFields();
        GenerateRandomFields();
    }

    void GenerateSolidFields() {
        for (int i = 0; i < SolidFields.Count; i++) {
            if (SolidFields[i].stats_range != Vector2.zero) {
                if (Duplicated(i))
                    Debug.Log(Name + ": " + SolidFields[i].type + " is duplicated.");
                else if (SolidFields[i].stats_range != Vector2.zero) {
                    float Stats = GenerateStats(E, SolidFields[i]);
                    E.Stats.Add(SolidFields[i].type, Stats);
                }
            }
        }
    }

    void GenerateRandomFields() {
        if (RandomFields.Choose == 0 ||RandomFields.Fields.Count == 0)
            return;
        else if(RandomFields.Choose > RandomFields.Fields.Count) {
            Debug.Log("RandomFields set up not correct!");
            return;
        }

        List<int> FieldsToChoose_Indexes = new List<int>();
        List<int> FieldsToGen_Indexes = new List<int>();
        for (int f_infex = 0; f_infex< RandomFields.Fields.Count;f_infex++)
            FieldsToChoose_Indexes.Add(f_infex);
        for(int i = 0; i < RandomFields.Choose; i++) {
            int AddFieldIndex = UnityEngine.Random.RandomRange(0, FieldsToChoose_Indexes.Count);
            FieldsToGen_Indexes.Add(FieldsToChoose_Indexes[AddFieldIndex]);
            FieldsToChoose_Indexes.Remove(FieldsToChoose_Indexes[AddFieldIndex]);
        }

        foreach(int f in FieldsToGen_Indexes) {
            float Stats = GenerateStats(E, RandomFields.Fields[f]);
            E.Stats.Add(RandomFields.Fields[f].type, Stats);
        }
    }

    int GetRandomIlvl(Vector2 ILvlRange) {
        return (int)Random.Range(ILvlRange.x, ILvlRange.y + 1);
    }

    Rarity GetRandomRarity(float RarityMod) {
        float R_Rate = UnityEngine.Random.value;
        if (R_Rate <= (RarityMod / 100) + ((float)RarityRate.Legendary / 100)) {
            return Rarity.Legendary;
        } else if (R_Rate <= (RarityMod / 100) + ((float)RarityRate.Mythic / 100)) {
            return Rarity.Mythic;
        } else if (R_Rate <= (RarityMod / 100) + ((float)RarityRate.Perfect / 100)) {
            return Rarity.Perfect;
        } else if(R_Rate <= (RarityMod / 100) + ((float)RarityRate.Fine / 100)) {
            return Rarity.Fine;
        } else {
            return Rarity.Common;
        }
    }

    bool Duplicated(int index) {
        for(int i =0; i < SolidFields.Count; i++) {
            if (i == index)
                continue;
            else
                return SolidFields[i].type == SolidFields[index].type;
        }
        return false;
    }

    private void InstantiateEquipmentData() {
        E = new Equipment();
        E.Name = Name;
        E.Class = Class;
        E.EquipType = EquipType;
        E.Set = Set;
        E.Description = Description;
    }

    float GenerateStats(Equipment _E, EquipmentField _EF) {
        float LowerBound = Mathf.Max(_EF.stats_range.y * (float)_E.Rarity / 10, _EF.stats_range.x);
        //Debug.Log(LowerBound);
        float HigherBound = _E.Rarity == Rarity.Legendary ? _EF.stats_range.y : (_EF.stats_range.y - _EF.stats_range.x) * (((float)_E.Rarity + 2) / 2) * 0.2f + _EF.stats_range.x;
        //Debug.Log(HigherBound);
        float Stats = Random.Range(LowerBound, HigherBound);
        if (_E.Itemlvl < Patch.MaxItemlvl)
            Stats = (Stats / ((Patch.MaxItemlvl - Itemlvl) * Patch.Itemlvl_Scale));
        if ((int)StatsType.DEFENSE <= (int)_EF.type && (int)_EF.type <= (int)StatsType.MANA_REGEN)
            Stats = (float)System.Math.Round(Stats, 1);
        else
            Stats = (float)System.Math.Round(Stats, 0);
        return Stats;
    }
}
