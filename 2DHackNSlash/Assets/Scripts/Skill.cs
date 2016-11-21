using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour {
    [HideInInspector]
    public SkillData SD; //For runtime data fetching

    //For designing purpose only
    public string Name;
    public string Description;

    protected ObjectController OC;

    protected virtual void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Skill");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Skill"), LayerMask.NameToLayer("Loot"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Skill"), LayerMask.NameToLayer("LootBox"));
        SD = ScriptableObject.CreateInstance<SkillData>();
        SD.Name = Name;
        SD.Description = Description;
    }

    public virtual void InitSkill(int lvl) {
        SD.lvl = lvl;
        OC = transform.parent.parent.GetComponent<ObjectController>();
    }

    // Use this for initialization
    protected virtual void Start () {
	    
	}

    // Update is called once per frame
    protected virtual void Update () {
	
	}

    public Sprite GetSkillIcon() {
        return GetComponent<Image>().sprite;
    }

    public ObjectController GetOC() {
        return OC;
    }
}
