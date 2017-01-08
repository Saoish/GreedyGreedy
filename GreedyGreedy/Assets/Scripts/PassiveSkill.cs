using UnityEngine;
using System.Collections;

public abstract class PassiveSkill : Skill {



    protected override void Awake() {
        base.Awake();
    }

    public override void Delete() {        
        Destroy(gameObject);
    }

public override void InitSkill(ObjectController OC, int lvl = 0) {
        base.InitSkill(OC,lvl);        
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public abstract void ApplyPassive();
}
