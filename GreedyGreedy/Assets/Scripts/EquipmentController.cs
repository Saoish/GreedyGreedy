using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GreedyNameSpace;

public class EquipmentController : MonoBehaviour {
    public List<StatsRangeField> SolidFields;
    public NChooseNFields SpecialFields;    

    [HideInInspector]
    public Equipment E;

    private Animator Anim;
    // Use this for initialization
    void Awake() {       
        Anim = GetComponent<Animator>();
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }


    public void EquipUpdate(Player PC) {
        if (E.EquipType == EQUIPTYPE.Trinket) {//No update for trinket for now
            return;
        }
        Anim.SetInteger("Direction", PC.Direction);
        if (PC.Attacking) {
            Anim.speed = PC.GetAttackAnimSpeed();
        } else {
            Anim.speed = PC.GetMovementAnimSpeed();
        }
        if (E.EquipType == EQUIPTYPE.Weapon) {//Weapon animation speed is controlled by AttackCollider
            if (PC.AttackVector != Vector2.zero) {
                Anim.SetBool("IsAttacking",true);
                Anim.SetInteger("Direction", PC.Direction);
            } else {
                Anim.SetBool("IsAttacking", false);
            }
            Anim.SetBool("Casting", PC.Casting);
            SpriteRenderer WeaponSpriteRenderer = GetComponent<SpriteRenderer>();
            if (PC.Direction == 3) {
                WeaponSpriteRenderer.sortingLayerName = SortingLayer.Object;
                WeaponSpriteRenderer.sortingOrder = -1;
            } else {
                WeaponSpriteRenderer.sortingLayerName = SortingLayer.Equip;
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
        //equipPrefab.transform.position = parent.transform.position + equipPrefab.transform.position;
        Destroy(equipPrefab.transform.Find("LootBox").gameObject);
        Destroy(equipPrefab.transform.Find("Icon").gameObject);
        equipPrefab.transform.parent = parent;
        equipPrefab.transform.localPosition = Vector3.zero;
        equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Equip;
        if (E.EquipType == GreedyNameSpace.EQUIPTYPE.Weapon) {
            equipPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        equipPrefab.GetComponent<SpriteRenderer>().enabled = true;
        equipPrefab.GetComponent<Animator>().enabled = true;
        return equipPrefab;
    }

    static public GameObject ObtainPrefab(Equipment E,Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        equipPrefab.GetComponent<EquipmentController>().E = E;
        Destroy(equipPrefab.GetComponent<Rigidbody2D>());
        Destroy(equipPrefab.GetComponent<BoxCollider2D>());
        equipPrefab.name = E.Name;
        //equipPrefab.transform.position = parent.transform.position + equipPrefab.transform.position;
        Destroy(equipPrefab.transform.Find("LootBox").gameObject);
        Destroy(equipPrefab.transform.Find("Icon").gameObject);
        equipPrefab.transform.parent = parent.GetComponent<Player>().GetVisualHolderTransform();
        equipPrefab.transform.localPosition = Vector3.zero;
        equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Equip;
        if (E.EquipType == GreedyNameSpace.EQUIPTYPE.Weapon) {
            //equipPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if (equipPrefab.transform.Find("MeleeAttackCollider") != null) {
                parent.GetComponent<Player>().SwapMeleeAttackCollider(equipPrefab.transform.Find("MeleeAttackCollider"));
            }
        }
        //} else if (E.Type == "Trinket")
        //    equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Equip;
        //else
        //    equipPrefab.GetComponent<SpriteRenderer>().sortingLayerName = Layer.Equip;
        equipPrefab.GetComponent<SpriteRenderer>().enabled = true;
        equipPrefab.GetComponent<Animator>().enabled = true;
        return equipPrefab;
    }

    static public GameObject ObtainEquippedIcon(Equipment E, Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        GameObject equipIcon = equipPrefab.transform.Find("Icon").gameObject;
        E.Description = equipPrefab.GetComponent<EquipmentController>().E.Description;
        equipPrefab.GetComponent<EquipmentController>().E = E;
        equipIcon.SetActive(true);
        equipIcon.name = E.Name;
        equipIcon.transform.position = parent.transform.position + equipIcon.transform.position;
        equipIcon.transform.parent = parent;
        Destroy(equipPrefab);
        return equipIcon;
    }

    static public GameObject ObtainInventoryIcon(Equipment E, Transform parent) {
        GameObject equipPrefab = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name)) as GameObject;
        GameObject equipIcon = equipPrefab.transform.Find("Icon").gameObject;
        E.Description = equipPrefab.GetComponent<EquipmentController>().E.Description;
        equipPrefab.GetComponent<EquipmentController>().E = E;
        equipIcon.SetActive(true);
        equipIcon.name = E.Name;
        equipIcon.transform.position = parent.transform.position + equipIcon.transform.position;
        equipIcon.transform.parent = parent;
        equipIcon.transform.localScale = new Vector2(500, 500);
        Destroy(equipPrefab);
        return equipIcon;
    }

    public void InstantiateLoot(Vector3 Position, Vector2 ILvlRange, float RarityMod) {
        GameObject Loot = Instantiate(Resources.Load("EquipmentPrefabs/" + E.Name), Position, gameObject.transform.rotation) as GameObject;
        SetUpLootObject(Loot,ILvlRange,RarityMod);
    }

    void SetUpLootObject(GameObject Loot, Vector2 ILvlRange, float RarityMod) {
        Loot.layer = LayerMask.NameToLayer(CollisionLayer.Loot);
        //Loot.transform.Find("LootBox").gameObject.layer = LayerMask.NameToLayer(CollisionLayer.Interaction);
        Loot.name = E.Name;
        Destroy(Loot.transform.Find("Icon").gameObject);
        Loot.GetComponent<EquipmentController>().RandomlizeEquip(ILvlRange, RarityMod);
        Loot.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayer.Loot; //Subject to change
        Loot.GetComponent<Collider2D>().enabled = true;
        Loot.transform.Find("LootBox").gameObject.SetActive(true);
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
                    Debug.Log(E.Name + ": " + SolidFields[i].type + " is duplicated.");
                else if (SolidFields[i].stats_range != Vector2.zero) {
                    float Stats = GenerateStats(E, SolidFields[i]);
                    E.Stats.Add(SolidFields[i].type, Stats);
                }
            }
        }
    }

    void GenerateRandomFields() {
        if (SpecialFields.Choose == 0 ||SpecialFields.Fields.Count == 0)
            return;
        else if(SpecialFields.Choose > SpecialFields.Fields.Count) {
            Debug.Log("RandomFields set up not correct!");
            return;
        }

        List<int> FieldsToChoose_Indexes = new List<int>();
        List<int> FieldsToGen_Indexes = new List<int>();
        for (int f_infex = 0; f_infex< SpecialFields.Fields.Count;f_infex++)
            FieldsToChoose_Indexes.Add(f_infex);
        for(int i = 0; i < SpecialFields.Choose; i++) {
            int AddFieldIndex = UnityEngine.Random.RandomRange(0, FieldsToChoose_Indexes.Count);
            FieldsToGen_Indexes.Add(FieldsToChoose_Indexes[AddFieldIndex]);
            FieldsToChoose_Indexes.Remove(FieldsToChoose_Indexes[AddFieldIndex]);
        }

        foreach(int f in FieldsToGen_Indexes) {
            float Stats = GenerateStats(E, SpecialFields.Fields[f]);
            E.Stats.Add(SpecialFields.Fields[f].type, Stats);
        }
    }

    int GetRandomIlvl(Vector2 ILvlRange) {
        return (int)Random.Range(ILvlRange.x, ILvlRange.y + 1);
    }

    RARITY GetRandomRarity(float RarityMod) {
        float R_Rate = UnityEngine.Random.value;
        if (R_Rate <= (RarityMod / 100) + ((float)RARITYRATE.Mythic / 100)) {
            return RARITY.Mythic;
        } else if (R_Rate <= (RarityMod / 100) + ((float)RARITYRATE.Legendary / 100)) {
            return RARITY.Legendary;
        } else if (R_Rate <= (RarityMod / 100) + ((float)RARITYRATE.Pristine / 100)) {
            return RARITY.Pristine;
        } else if(R_Rate <= (RarityMod / 100) + ((float)RARITYRATE.Fine / 100)) {
            return RARITY.Fine;
        } else {
            return RARITY.Common;
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

    float GenerateStats(Equipment _E, StatsRangeField _EF) {
        float LowerBound = Mathf.Max(_EF.stats_range.y * (float)_E.Rarity / 10, _EF.stats_range.x);
        //Debug.Log(LowerBound);
        float HigherBound = _E.Rarity == RARITY.Mythic ? _EF.stats_range.y : (_EF.stats_range.y - _EF.stats_range.x) * (((float)_E.Rarity + 2) / 2) * 0.2f + _EF.stats_range.x;
        //Debug.Log(HigherBound);
        float Stats = Random.Range(LowerBound, HigherBound);
        if (_E.Itemlvl < Patch.MaxItemlvl)
            Stats = (Stats / ((Patch.MaxItemlvl - _E.Itemlvl) * Patch.Itemlvl_Scale));
        if ((int)STATSTYPE.DEFENSE <= (int)_EF.type && (int)_EF.type <= (int)STATSTYPE.ESSENSE_REGEN)
            Stats = (float)System.Math.Round(Stats, 1);
        else
            Stats = (float)System.Math.Round(Stats, 0);
        return Stats;
    }
}
