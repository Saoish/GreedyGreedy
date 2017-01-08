using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour {
    //For designing purpose only
    public string Name;
    [HideInInspector]
    public string Description;
    [HideInInspector]
    public int lvl;

    protected ObjectController OC;

    protected string highlight = "orange";
    public abstract void GenerateDescription();

    protected virtual void Awake() {
        gameObject.layer = LayerMask.NameToLayer(CollisionLayer.Skill);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.Skill), LayerMask.NameToLayer(CollisionLayer.Loot));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.Skill), LayerMask.NameToLayer(CollisionLayer.Interaction));
    }

    public Skill Instantiate() {
        GameObject Skill_OJ = Instantiate(Resources.Load("SkillPrefabs/" +Name)) as GameObject;
        Skill_OJ.name = Name;
        return Skill_OJ.GetComponent<Skill>();
    }

    public abstract void Delete();

    public virtual void InitSkill(ObjectController OC,int lvl = 0) {
        this.lvl = lvl;
        this.OC = OC;
        transform.position = OC.Skills_T().position;
        transform.SetParent(OC.Skills_T());
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
