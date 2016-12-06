using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour {
    [HideInInspector]
    public SkillData SD; //For runtime data fetching, don't use it on UI update

    //For designing purpose only
    public string Name;
    public string Description;

    protected ObjectController OC;

    protected virtual void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Skill");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Skill"), LayerMask.NameToLayer("Loot"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Skill"), LayerMask.NameToLayer("LootBox"));
        //SD = ScriptableObject.CreateInstance<SkillData>();
        SD = new SkillData();
        SD.Name = Name;
        SD.Description = Description;
        //if (transform.GetComponent<SpriteRenderer>())
        //    transform.GetComponent<SpriteRenderer>().sortingOrder = Layer.Skill;
        //if (transform.GetComponent<ParticleSystem>())
        //    transform.GetComponent<ParticleSystemRenderer>().sortingOrder = Layer.Skill;
    }

    public virtual void InitSkill(ObjectController OC,int lvl) {
        SD.lvl = lvl;
        this.OC = OC;
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
