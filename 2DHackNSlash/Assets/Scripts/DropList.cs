using UnityEngine;
using System.Collections;

public class DropList : MonoBehaviour {
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
        foreach (var i in Drops) {
            if (!i.Item)
                continue;
            else if (UnityEngine.Random.value <= (i.Rate / 100)) {
                int RandomOffsetIndex;
                do {
                    RandomOffsetIndex = UnityEngine.Random.Range(0, SpawnOffsets.Length);
                } while (RandomOffsetIndex == LastOffsetIndex);
                i.Item.GetComponent<EquipmentController>().InstantiateLootAt(transform.position + (Vector3)SpawnOffsets[RandomOffsetIndex]);
                LastOffsetIndex = RandomOffsetIndex;
            }
        }
    }
}
