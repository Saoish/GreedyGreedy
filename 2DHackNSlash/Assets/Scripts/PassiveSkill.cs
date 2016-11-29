using UnityEngine;
using System.Collections;

public abstract class PassiveSkill : Skill {



    protected override void Awake() {
        base.Awake();
    }

    public override void InitSkill(ObjectController OC, int lvl) {
        base.InitSkill(OC,lvl);
        gameObject.name = SD.Name;
        transform.position = OC.T_Passives.position;
        transform.SetParent(OC.T_Passives);
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public abstract void ApplyPassive();
}
