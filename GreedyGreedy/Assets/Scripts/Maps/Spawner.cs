using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
    public List<GameObject> SpawnList;

    protected int LastOffsetIndex;

    protected Vector2[] SpawnOffsets = new Vector2[] {
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
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}

    public virtual GameObject Spawn() {
        GameObject temp = Instantiate(SpawnList[0]);
        int RandomOffsetIndex;
        do {
            RandomOffsetIndex = UnityEngine.Random.Range(0, SpawnOffsets.Length);
        } while (RandomOffsetIndex == LastOffsetIndex);
        temp.transform.position = transform.position + (Vector3)SpawnOffsets[RandomOffsetIndex];
        LastOffsetIndex = RandomOffsetIndex;
        temp.name = temp.GetComponentInChildren<Monster>().GetName();
        return temp;
    }
}
