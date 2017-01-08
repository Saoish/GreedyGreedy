using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Debuff : MonoBehaviour {
    [HideInInspector]
    public float Duration = 0;

    protected ObjectController target;
    protected ObjectController applyer;

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

    virtual public void ApplyDebuff(ObjectController applyer,ObjectController target) {
        this.target = target;
        this.applyer = applyer;
        gameObject.transform.SetParent(target.Debuffs_T());
        gameObject.transform.localPosition = Vector3.zero;
    }

    public abstract void RemoveDebuff();
}
