using UnityEngine;
using System.Collections;
using GreedyNameSpace;

public class DropList : MonoBehaviour {
    public int Variation = 0; //0 will always drop the exact lvl as MainPlayer
    public float RarityMod = 0f;//How like you want to Top tier be dropped? (0-100)

    public Loot[] Drops;

    int LastOffsetIndex;

    Vector2[] SpawnOffsets = new Vector2[] {
        new Vector2(0,0),
        new Vector2(0.1f,0),
        new Vector2(0,0.1f),
        new Vector2(-0.1f,0),
        new Vector2(0,-0.1f),
        new Vector2(0.1f,0.1f),
        new Vector2(0.1f,-0.1f),
        new Vector2(-0.1f,0.1f),
        new Vector2(-0.1f,-0.1f)
    };

    public void SpawnLoots() {
        int CurrLvl = FindObjectOfType<MainPlayer>().Getlvl();        
        int min = CurrLvl - Variation < 1 ? 1 : CurrLvl - Variation;
        int max = CurrLvl + Variation > Patch.LvlCap ? Patch.LvlCap : CurrLvl + Variation;
        foreach (var i in Drops) {
            if (!i.Item)
                continue;
            else if (UnityEngine.Random.value <= (i.Rate / 100)) {
                int RandomOffsetIndex;
                do {
                    RandomOffsetIndex = UnityEngine.Random.Range(0, SpawnOffsets.Length);
                } while (RandomOffsetIndex == LastOffsetIndex);
                i.Item.GetComponent<EquipmentController>().InstantiateLoot(transform.position + (Vector3)SpawnOffsets[RandomOffsetIndex],new Vector2(min,max), RarityMod);
                LastOffsetIndex = RandomOffsetIndex;
            }
        }
    }
}
