using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Debuff : MonoBehaviour {
    [HideInInspector]
    public ModData MD;
    [HideInInspector]
    public float Duration = 0;

    protected float ModAmount;

    protected ObjectController target;


    protected virtual void Update() {
        if (target == null)
            return;
        else {
            if (Duration > 0)
                Duration -= Time.deltaTime;
            else if (Duration <= 0)
                RemoveDebuff();
        }
    }

    virtual public void ApplyDebuff(ModData MD,ObjectController target) {
        this.target = target;
        this.MD = MD;
        gameObject.transform.SetParent(target.transform.Find("Debuffs"));
        gameObject.transform.localPosition = Vector3.zero;
    }

    virtual public void ApplyDebuff(ModData MD, ObjectController target,Value debuff_dmg) {
        this.target = target;
        this.MD = MD;
        gameObject.transform.SetParent(target.transform.Find("Debuffs"));
        gameObject.transform.localPosition = Vector3.zero;
    }

    abstract protected void RemoveDebuff();
}
